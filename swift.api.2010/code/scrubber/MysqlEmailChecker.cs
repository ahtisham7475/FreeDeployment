using log4net;
using System;

namespace swift.api._2010.code.scrubber
{
    public class MysqlEmailChecker
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(MysqlEmailChecker));

        public string Email { get; set; }

        string[] Candidate;

        string _emailLocalPart;

        string _emailDomainPart;

        public MysqlEmailChecker(string emailId)
        {
            Email = emailId;
        }

        public EmailResponse Validate(string apiKey)
        {
            EmailResponse response = null;
            using (MysqlHandler handler = new MysqlHandler())
            {
                try
                {

                    if (!InputIsValid())
                    {
                        return new EmailResponse(Email, "Error", "InvalidEmailId");
                    }

                    try
                    {
                        var validator = new FakeEmailValidator();
                        var result = validator.ValidateEmail(Email);
                        _log.Info($"FakeEmailValidator: ${result.Log.Replace(Email, string.Empty)}");

                        if (!result.IsValid)
                        {
                            response = new EmailResponse(Email, "Bad", "FakeEmail");
                            return response;
                        }

                        response = new EmailResponse(Email, "Good", "OK‏");
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message, ex);
                        response = new EmailResponse(Email, "Bad", "UnhandledException");
                    }

                    int rs = handler.ValidateF(apiKey, _emailLocalPart, _emailDomainPart);
                    switch (rs)
                    {
                        case 1: response = new EmailResponse(Email, "Bad", "RoleAccount"); break;
                        case 2: response = new EmailResponse(Email, "Bad", "DisposableEmailAddress"); break;
                        case 3: response = new EmailResponse(Email, "Bad", "PubliclyAvailable"); break;
                        case 4: response = new EmailResponse(Email, "Bad", "EmailDomainBlacklist"); break;
                        case 6: response = new EmailResponse(Email, "Good", "OK‏"); break;
                        default: break;

                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    response = new EmailResponse(ex.ToString(), "Good", "OK");
                }
            }

            return response;
        }

        private bool InputIsValid()
        {
            try
            {
                Candidate = Email.Split("@".ToCharArray());
                if (Candidate.Length != 2 || Candidate[0] == "" || Candidate[1] == "")
                {
                    return false;
                }

                _emailLocalPart = Candidate[0];
                _emailDomainPart = Candidate[1];
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return false;
            }

            return true;
        }
    }
}