using KellermanSoftware.NetEmailValidation;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace swift.api.code.kellerman
{

    // This class is responsible for validating emails using Kellerman library.
    public class Kellerman
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Kellerman));

        // credentials
        private static readonly string USERNAME = "OZONE Internet Services 103120";
        private static readonly string KEY = "2;6F18FA127A5F217E77C857B19618A5EADB";
        private ConcurrentBag<Result> _results;

        private readonly string _email;

        private readonly ConcurrentDictionary<string, ConcurrentBag<string>> _statusCodes;

        public Kellerman(string email)
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
            _email = email;
            _results = new ConcurrentBag<Result>();

            _statusCodes = new ConcurrentDictionary<string, ConcurrentBag<string>>();
            _statusCodes["Valid"] = new ConcurrentBag<string>();
            _statusCodes["Invalid"] = new ConcurrentBag<string>();
            _statusCodes["Unknown"] = new ConcurrentBag<string>();
            _statusCodes["Error"] = new ConcurrentBag<string>();
        }

        public string Id { get; }

        // verify email using a single task and sequential exclusion check algorithm
        public KellermanStatus VerifyInSingleTask(ConcurrentDictionary<string, ConcurrentBag<string>> statusCodes)
        {
            KellermanStatus response = KellermanStatus.Invalid;
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = tokenSource.Token;

            try
            {
                Task task = Task.Factory.StartNew(() => SingleTaskAction(_email, tokenSource), cancelToken);

                Task[] tasks = new Task[1];
                tasks[0] = task;

                bool waitingComplete = false;
                waitingComplete = Task.WaitAll(tasks, Config.TASKS_TIMEOUT, cancelToken);
            }
            catch (Exception e)
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

            response = ReadFromConcurrentBag();
            PassStatusCodes(statusCodes);
            return response;
        }


        // email validation delegate
        private void SingleTaskAction(object state, CancellationTokenSource source)
        {
            string email = (string)state;

            if (email == null)
            {
                return;
            }

            List<Result> result;

            List<string> e = new List<string>
            {
                email
            };

            EmailValidation engine = GetNew();

            List<ValidationOptions> opt = new List<ValidationOptions>
            {
                ValidationOptions.BasicSyntax,
                ValidationOptions.TypoChecking,
                ValidationOptions.FakeEmailPatternMatcher,
                ValidationOptions.DisallowDisposableEmail,
                ValidationOptions.DisallowCurseWords
            };

            result = engine.ValidateList(e, opt);
            if (result != null && result.Count != 0)
            {
                _results.Add(result[0]);
                foreach (var item in result)
                {
                    _log.Info(item.Log?.ToLowerInvariant()?.Replace(email.ToLowerInvariant(), string.Empty));
                }
            }
        }

        // creates an new instance of validation engine
        private EmailValidation GetNew()
        {
            EmailValidation _engine = null;
            // we need to do this, cause the kellerman is not thread optimised
            //  and sometimes we got INI file exception
            while (_engine == null)
            {
                try { _engine = new EmailValidation(USERNAME, KEY); }
                catch (Exception) { _engine = null; }
                if (_engine != null)
                {
                    break;
                }
            }
            return _engine;
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
        private KellermanStatus ReadFromConcurrentBag()
        {
            KellermanStatus response = KellermanStatus.Invalid;
            try
            {
                if (_results != null)
                {
                    int valid = 0;
                    int invalid = 0;
                    foreach (Result res in _results)
                    {
                        if (res.IsValid)
                        {
                            valid++;
                            _statusCodes["Valid"].Add(res.Status.ToString());

                        }
                        else if (!res.IsValid)
                        {
                            invalid++;
                            _statusCodes["Invalid"].Add(res.Status.ToString());
                        }

                        else
                        {
                            _statusCodes["Unknown"].Add(res.Status.ToString());
                        }
                    }

                    if (invalid == 0)
                    {
                        response = KellermanStatus.Valid;
                    }
                    else
                    {
                        response = KellermanStatus.Invalid;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return response;
        }
    }
}