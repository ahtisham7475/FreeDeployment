using Cobisi.EmailVerify;
using swift.api.code.other;
using System;

namespace swift.api.code.cobisi
{
    // This class is responsible to manage information needed to verify an email
    //  using the Cobisi library
    public class CobisiEmailState
    {
        private ProxyInfo _proxy;
        
        public CobisiEmailState(string email, VerificationLevel level, ProxyInfo proxy)
        {
            Id1 = Guid.NewGuid().ToString().Replace("-", "");
            EmailAddress = email;
            VerificationLevel = level;
            if (proxy != null)
            {
                _proxy = new ProxyInfo
                {
                    Ip = proxy.Ip,
                    Name = proxy.Name,
                    Password = proxy.Password,
                    Port = proxy.Port,
                    UserName = proxy.UserName,
                    IsHttpConnect = proxy.IsHttpConnect,
                    HostFqdm = proxy.HostFqdm,
                    SenderAddress = proxy.SenderAddress
                };
            }

        }


        public string Id { get { return Id1; } }

        public string EmailAddress { get; set; }

        public VerificationLevel VerificationLevel { get; set; }        

        public string Id1 { get; }
    }
}