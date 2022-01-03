using log4net;
using swift.api._2010.code.scrubber;
using swift.api.code;
using swift.api.code.other;
using swift.api.code.token;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.UI;

namespace swift.api.ui
{

    // This class is responsible for presenting the application to the user
    //  If any of the required parameters, api key or email to be validated are missing
    //  the application returns an error page.
    public class Start : IRouteHandler
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Start));

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var logId = Guid.NewGuid().ToString().Replace("-", string.Empty);
            _log.Info($"SPEED!! Info-{logId}");

            // get parameters from query
            var email = requestContext.RouteData.Values["Email"] as string;
            var apiKey = requestContext.RouteData.Values["Account"] as string;

            // the application also will reject any parameter which is an empty string
            //  or a string contains only white spaces
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(apiKey) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(apiKey))
            {
                return BuildManager.CreateInstanceFromVirtualPath("~/Error.aspx", typeof(Page)) as Page;
            }

            var apiKeyManager = new ApiKeyManager();
            var statusCodes = new ConcurrentDictionary<string, ConcurrentBag<string>>();
            statusCodes["Error"] = new ConcurrentBag<string>();
            statusCodes["Unknown"] = new ConcurrentBag<string>();

            var sw = new Stopwatch();
            sw.Start();

            var token = apiKeyManager.Authenticate(apiKey, out bool succes, statusCodes);

            sw.Stop();
            _log.Info($"SPEED!! Info-{logId} check token: {sw.Elapsed.TotalSeconds}s");

            if (email.ToLower() == "TokenInfo".ToLower())
            {
                if (succes)
                {
                    if (token != null)
                    {
                        TokenResult result = new TokenResult
                        {
                            ActualQueryCount = token.CurrentQuota,
                            InitalQueryCount = token.InitialQuota,
                            Token = token.Key,
                            Status = "Ok"
                        };
                        return new TokenHandler(result);
                    }

                    else
                    {
                        TokenResult result = new TokenResult
                        {
                            ActualQueryCount = 0,
                            InitalQueryCount = -1,
                            Token = apiKey,
                            Status = StatusCode.INVALID_TOKEN
                        };
                        return new TokenHandler(result);
                    }
                }
                else
                {
                    string sts = "";
                    foreach (string s in statusCodes["Error"])
                    {
                        sts += s + ",";
                    }

                    TokenResult result = new TokenResult
                    {
                        ActualQueryCount = 0,
                        InitalQueryCount = -1,
                        Token = apiKey,
                        Status = sts
                    };

                    return new TokenHandler(result);
                }
            }
            else
            {
                if (!succes)
                {
                    return new ResponseHandler(new OutputResult
                    {
                        Address = email,
                        Status = "InvalidToken",
                        StatusCode = "Unknown"
                    });
                }

                if (token.CurrentQuota <= 0)
                {
                    return new ResponseHandler(new OutputResult
                    {
                        Address = email,
                        Status = "NoMoreQueries",
                        StatusCode = "Unknown"
                    });
                }

                var updateQuotaTask = Task.Run(() =>
                {
                    var swTask = new Stopwatch();
                    swTask.Start();

                    apiKeyManager.UpdateQuota(token);

                    swTask.Stop();
                    _log.Info($"SPEED!! Info-{logId} update quota: {sw.Elapsed.TotalSeconds}s");
                });

                var verificationEngineTask = Task.Run(() =>
                {
                    var swTask = new Stopwatch();
                    swTask.Start();

                    var verifiedResult = new VerificationEngine().Verify(email, apiKey);

                    swTask.Stop();
                    _log.Info($"SPEED!! Info-{logId} VerificationEngine SMTP/Public: {sw.Elapsed.TotalSeconds}s");

                    return verifiedResult;
                });

                sw.Restart();

                var emailChecker = new MysqlEmailChecker(email);
                var response = emailChecker.Validate(apiKey);

                sw.Stop();
                _log.Info($"SPEED!! Info-{logId} check scrubber email: {sw.Elapsed.TotalSeconds}s");

                if (!response.StatusCode.Equals("OK", StringComparison.InvariantCultureIgnoreCase))
                {
                    updateQuotaTask.GetAwaiter().GetResult();
                    return new ResponseHandler(new OutputResult
                    {
                        Address = email,
                        StatusCode = response.StatusCode,
                        Status = response.Status
                    });
                }

                updateQuotaTask.GetAwaiter().GetResult();
                var outputResult = verificationEngineTask.GetAwaiter().GetResult();

                return new ResponseHandler(outputResult);
            }
        }
    }
}