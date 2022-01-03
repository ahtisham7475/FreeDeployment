using KellermanSoftware.NetEmailValidation;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace swift.api._2010.code.scrubber
{
    public class FakeEmailValidator
    {
        static readonly string VALIDATION_MESSAGE = "Catchall Email Domain";
        EmailValidation emailValidation = new EmailValidation("OZONE Internet Services 103120", "2;6F18FA127A5F217E77C857B19618A5EADB");

        static readonly object _object = new object();

        public FakeResult ValidateEmail(string email_address)
        {
            var log = new StringBuilder();
            var result = emailValidation.ValidEmail(email_address, emailValidation.NoConnectOptions);

            log.Append(result.Log);
            if (result.IsValid)
            {
                var res = SecondRule(email_address);
                if (res)
                {
                    log.Append("\nPass : Bad Words List");
                    res = ThirdRule(email_address);
                    if (res)
                    {
                        log.Append("\nPass : Max Digits Rule");
                        res = FourthRule(email_address);
                        if (res)
                        {
                            log.Append("\nPass : Max Length Rule");
                        }
                        else
                        {
                            log.Append("\nFailed : Max Length Rule");
                            return new FakeResult(log.ToString(), false);
                        }
                    }
                    else
                    {
                        log.Append("\nFailed : Max Digits Rule");
                        return new FakeResult(log.ToString(), false);
                    }
                }
                else
                {
                    log.Append("\nFailed : Bad Words List");
                    return new FakeResult(log.ToString(), false);
                }
            }
            else
            {
                return new FakeResult(log.ToString(), false);
            }

            return new FakeResult(log.ToString(), true);
        }

        private static bool SecondRule(string email_address)
        {
            var asm = Assembly.GetExecutingAssembly();
            var xmlStream = asm.GetManifestResourceStream("swift.api._2010.code.scrubber.data.badwords.txt");
            // read all lines of file
            string line = null;
            // get local part of email address
            if (!email_address.Contains("@"))
            {
                return false;
            }

            string email_local = email_address.Split('@')[0];
            StreamReader sr = new StreamReader(xmlStream);
            // read all lines of file
            while ((line = sr.ReadLine()) != null)
            {
                string _line = line.Trim();
                // if local part is equal to some blocked word or it contains the blocked word
                if (!string.IsNullOrEmpty(_line) && (email_local.ToLower().Equals(_line.ToLower()) || email_local.ToLower().Contains(_line.ToLower())))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ThirdRule(string email_address)
        {
            // get local part of email address
            if (!email_address.Contains("@"))
            {
                return false;
            }
            string email_local = email_address.Split('@')[0];
            char[] tokens = email_local.ToCharArray();
            int char_count = 0;
            foreach (char t in tokens)
            {
                if (char.IsNumber(t))
                {
                    char_count++;
                }
                else
                {
                    char_count = 0;
                }

                if (char_count > 5)
                {
                    break;
                }
            }
            return char_count > 5 ? false : true;
        }

        private static bool FourthRule(string email_address)
        {
            // get local part of email address
            if (!email_address.Contains("@"))
            {
                return false;
            }
            string email_local = email_address.Split('@')[0];

            return email_local.Length > 35 ? false : true;
        }     
    }
}