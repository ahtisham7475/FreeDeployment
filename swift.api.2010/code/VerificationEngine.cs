using swift.api.code.cobisi;
using swift.api.code.kellerman;
using swift.api.code.other;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;

namespace swift.api.code
{
    // Use this class to validate emails, by calling the Verify method
    public class VerificationEngine
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public OutputResult Verify(string email, string key)
        {
            // Stupid fixed for VerifyWithSmtp(email, key), keep the address because the email set to null.
            var address = new MailAddress(email);

            var outputResult = VerifyWithSmtp(email, key);

            if (outputResult.Status.Equals("Unknown", StringComparison.InvariantCultureIgnoreCase)
                || outputResult.StatusCode.Equals("Unknown", StringComparison.InvariantCultureIgnoreCase))
            {
                var domains = new[] { "Yahoo", "Gmail", "Hotmail", "AOL" };
                if (domains.Any(d => address.Host.StartsWith(d, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var response = _httpClient.GetAsync($"https://publicmailverify.azurewebsites.net/api/Verify/{email}/Check").GetAwaiter().GetResult();
                    var output = response.Content.ReadAsAsync<OutputResult>().GetAwaiter().GetResult();
                    if (output.Status.Equals("Mailbox do not exists", StringComparison.InvariantCultureIgnoreCase))
                    {
                        output.StatusCode = "Invalid";
                    }

                    if (output.Status.Equals("Mailbox exists", StringComparison.InvariantCultureIgnoreCase))
                    {
                        output.StatusCode = "Valid";
                    }

                    return output;
                }
            }

            return outputResult;
        }

        private OutputResult VerifyWithSmtp(string email, string key)
        {
            List<string> codes = new List<string>();
            OutputResult response = new OutputResult
            {
                Address = email
            };

            // set the statuses bag
            ConcurrentDictionary<string, ConcurrentBag<string>> statusCodes = new ConcurrentDictionary<string, ConcurrentBag<string>>();
            statusCodes["Valid"] = new ConcurrentBag<string>();
            statusCodes["Invalid"] = new ConcurrentBag<string>();
            statusCodes["Unknown"] = new ConcurrentBag<string>();
            statusCodes["Error"] = new ConcurrentBag<string>();

            // otherwise

            KellermanStatus ks = new Kellerman(email).VerifyInSingleTask(statusCodes);

            if (ks == KellermanStatus.Valid)
            {
                CobisiStatus cs = new CobisiSingleEmailVerifier(email).Verify(statusCodes);

                // final result management
                switch (cs)
                {
                    case CobisiStatus.Invalid:

                        // manage status codes
                        foreach (string code in statusCodes["Invalid"])
                        {
                            codes.Add(code);
                        }

                        response.Status = CobisiUtil.StatusToString(CobisiStatus.Invalid);
                        response.StatusCode = GetStatusCode(codes);
                        break;
                    case CobisiStatus.Valid:

                        // manage status codes
                        foreach (string code in statusCodes["Valid"])
                        {
                            codes.Add(code);
                        }

                        response.Status = CobisiUtil.StatusToString(CobisiStatus.Valid);
                        response.StatusCode = GetStatusCode(codes);
                        break;
                    case CobisiStatus.Unknown:

                        foreach (string code in statusCodes["Unknown"])
                        {
                            codes.Add(code);
                        }

                        foreach (string error in statusCodes["Error"])
                        {
                            codes.Add(error);
                        }

                        response.Status = CobisiUtil.StatusToString(CobisiStatus.Unknown);
                        response.StatusCode = StatusCode.MAILBOX_VALIDATION_ERROR;// GetStatusCode(codes);
                        break;
                }

            }

            // if kellerman check returns invalid
            else
            {
                // manage status codes
                foreach (string code in statusCodes["Invalid"])
                {
                    codes.Add(code);
                }

                response.Status = CobisiUtil.StatusToString(CobisiStatus.Invalid);
                response.StatusCode = GetStatusCode(codes);
            }

            return response;
        }

        // get the status codes and computes
        private string GetStatusCode(List<string> codes)
        {
            string result = "";

            List<string> cc = new List<string>();

            foreach (string code in codes)
            {
                bool exists = false;
                string ccc = StatusCode.GetOutputStatusCode(code);

                for (int i = 0; i < cc.Count; i++)
                {
                    if (ccc == cc[i])
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    cc.Add(ccc);
                }
            }


            if (cc != null)
            {
                for (int i = 0; i < cc.Count; i++)
                {
                    if (i == (cc.Count - 1))
                    {
                        result += cc[i];
                    }
                    else
                    {
                        result += (cc[i] + ", ");
                    }
                }
            }
            return (result == "") ? StatusCode.MAILBOX_VALIDATION_ERROR : result;
        }
    }
}