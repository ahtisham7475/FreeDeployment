using Cobisi.EmailVerify;
using log4net;
using swift.api.code.other;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace swift.api.code.cobisi
{
    // This class is responsible for validating one single email address
    //  using the cobisi library
    public class CobisiSingleEmailVerifier
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(CobisiSingleEmailVerifier));

        private ConcurrentBag<CobisiResult> _results;

        private readonly string _id;

        private readonly string _email;

        private readonly ConcurrentDictionary<string, ConcurrentBag<string>> _statusCodes;

        public CobisiSingleEmailVerifier(string email)
        {
            _id = Guid.NewGuid().ToString().Replace("-", "");
            _results = new ConcurrentBag<CobisiResult>();
            _email = email;

            _statusCodes = new ConcurrentDictionary<string, ConcurrentBag<string>>();
            _statusCodes["Valid"] = new ConcurrentBag<string>();
            _statusCodes["Invalid"] = new ConcurrentBag<string>();
            _statusCodes["Unknown"] = new ConcurrentBag<string>();
            _statusCodes["Error"] = new ConcurrentBag<string>();
        }

        // checks the email by specified level
        public CobisiStatus Verify(ConcurrentDictionary<string, ConcurrentBag<string>> statusCodes, VerificationLevel level = VerificationLevel.CatchAll)
        {
            CobisiStatus response;

            // start verifying the email using the cobisi library
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = tokenSource.Token;
            List<Task> tasks = new List<Task>();
            List<string> log = new List<string>();
            string stateId = "";

            try
            {
                CobisiEmailState state = new CobisiEmailState(_email, level, null);
                stateId = state.Id;
                Task task = Task.Factory.StartNew(() => Action(state, tokenSource), cancelToken);
                tasks.Add(task);

                bool waitingComplete = false;
                waitingComplete = Task.WaitAll(tasks.ToArray(), Config.TASKS_TIMEOUT, cancelToken);
            }
            catch(Exception e)
            {
                _log.Error(e.Message, e);
            }          
            finally
            {
                if (!tokenSource.IsCancellationRequested)
                {
                    tokenSource.Cancel();
                }

                tokenSource.Dispose();
            }

            try
            {
                response = ReadFromConcurrentBag();
                PassStatusCodes(statusCodes);
                return response;
            }
            catch (Exception)
            {
                PassStatusCodes(statusCodes);
                return CobisiStatus.Unknown;
            }
        }

        // action to perform over email
        private void Action(CobisiEmailState es, CancellationTokenSource source)
        {
            CobisiResult result = new CobisiResult();
            try
            {
                CheckCancelation(source);
                if (es == null)
                {
                    return;
                }

                if (string.IsNullOrEmpty(es.EmailAddress) || string.IsNullOrWhiteSpace(es.EmailAddress))
                {
                    return;
                }

                CheckCancelation(source);
                EmailVerifier verifier = new EmailVerifier
                {
                    AllowComments = Config.ALLOW_COMMENTS,
                    AllowDomainLiterals = Config.ALLOW_DOMAIN_LITERALS,
                    AllowFoldingWhiteSpaces = Config.ALLOW_FOLDING_SPACES,
                    AllowInternationalDomainNames = Config.ALLOW_INTERNATIONAL_DOMAIN_NAMES,
                    AllowInternationalMailboxNames = Config.ALLOW_INTERNATIONAL_MAILBOX_NAMES,
                    AllowQuotedStrings = Config.ALLOW_QUOTED_STRINGS,
                    DnsQueryMaxRetriesCount = Config.DNS_MAX_RETRIES,
                    DnsQueryTimeout = Config.DNS_QUERY_TIMEOUT,
                    MailboxInspectionTimeout = Config.MAILBOX_INSPECTION_TIMEOUT,
                    SameDomainConnectionDelay = Config.SAME_DOMAIN_CONNECTION_DELAY,
                    SmtpConnectionTimeout = Config.SMTP_CONNECTION_TIMEOUT
                };

                verifier.DnsServers.Add(IPAddress.Parse("8.8.8.8"));
                verifier.DnsServers.Add(IPAddress.Parse("208.67.222.222"));
                verifier.DnsServers.Add(IPAddress.Parse("64.237.56.227"));
                verifier.DnsServers.Add(IPAddress.Parse("66.55.135.139"));

                verifier.SmtpConnectionTimeout = new TimeSpan(0, 0, 0, 30, 0);
                verifier.HttpConnectionTimeout = new TimeSpan(0, 0, 0, 30, 0);

                CheckCancelation(source);
                VerificationResult r = verifier.Verify(es.EmailAddress, es.VerificationLevel);

                result.EmailAddress = es.EmailAddress;
                result.Level = es.VerificationLevel;
                result.Result = r;

                if (r.Status != VerificationStatus.Success)
                {
                    result.Log.Add(r.ToString());
                    foreach (var e in r.Exceptions)
                    {
                        result.Log.Add(e?.ToString());
                    }
                }

                WriteResultToDictionary(result, source);
            }
            catch (Exception e)
            {
                result.Log.Add("Interruption exception: " + e.Message);
            }

            var domain = string.Empty;
            var name = string.Empty;
            try
            {
                var address = new MailAddress(es.EmailAddress);
                name = address.User;
                domain = address.Host;
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }

            _log.Error($"Domain: @{domain}");

            if (result.Log?.Any() == true)
            {
                _log.Error(string.Join(Environment.NewLine, result.Log)?.ToLowerInvariant()?.Replace(es.EmailAddress.ToLowerInvariant(), string.Empty));
            }
        }

        // write the current result to dictionary
        private void WriteResultToDictionary(CobisiResult result, CancellationTokenSource source)
        {
            if (_results != null)
            {
                // add result to collection
                _results.Add(result);

                // we need to check for success or insuccess, if we reach it
                //  we cancel all other tasks
                if (CobisiUtil.GetStatus(result.Result) != CobisiStatus.Unknown)
                {
                    // cancel the threads
                    if (source != null) { try { source.Cancel(); } catch (Exception) { } }
                }

            }
            else
            {
                // Ths Email has been processed by some other thread
                if (source != null) { try { source.Cancel(); } catch (Exception) { } }
            }
        }

        // checks for canceling signals
        private static void CheckCancelation(CancellationTokenSource source)
        {
            try
            {
                if (source.Token.IsCancellationRequested == true)
                {
                    source.Token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception) { }
        }

        // pass over the statuses whitout destroying the target
        private void PassStatusCodes(ConcurrentDictionary<string, ConcurrentBag<string>> target)
        {
            foreach (string valid in _statusCodes["Valid"])
            {
                target["Valid"].Add(valid);
            }

            foreach (string valid in _statusCodes["Invalid"])
            {
                target["Invalid"].Add(valid);
            }

            foreach (string valid in _statusCodes["Unknown"])
            {
                target["Unknown"].Add(valid);
            }

            foreach (string valid in _statusCodes["Error"])
            {
                target["Error"].Add(valid);
            }
        }

        // read results from collection
        private CobisiStatus ReadFromConcurrentBag()
        {
            CobisiStatus response = CobisiStatus.Unknown;
            try
            {
                if (_results != null)
                {
                    int valid = 0;
                    int invalid = 0;
                    foreach (CobisiResult res in _results)
                    {
                        if (CobisiUtil.GetStatus(res.Result) == CobisiStatus.Valid)
                        {
                            valid++;
                            _statusCodes["Valid"].Add(res.Result.Status.ToString());
                        }

                        if (CobisiUtil.GetStatus(res.Result) == CobisiStatus.Invalid)
                        {
                            invalid++;
                            _statusCodes["Invalid"].Add(res.Result.Status.ToString());
                        }

                        if (CobisiUtil.GetStatus(res.Result) == CobisiStatus.Unknown)
                        {
                            _statusCodes["Unknown"].Add(res.Result.Status.ToString());
                        }
                    }

                    if (valid != 0)
                    {
                        response = CobisiStatus.Valid;
                    }
                    else
                    {
                        if (invalid != 0)
                        {
                            response = CobisiStatus.Invalid;
                        }
                        else
                        {
                            response = CobisiStatus.Unknown;
                        }
                    }
                }
            }
            catch (Exception) { }

            return response;
        }
    }
}