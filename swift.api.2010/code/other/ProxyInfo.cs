using System;
using System.Collections.Generic;

namespace swift.api.code.other
{
    // This class is responsible for managing proxies
    public class ProxyInfo
    {

        // list of available proxy configurations
        private static List<ProxyInfo> proxies = new List<ProxyInfo>();

        private static readonly int _lastUsedProxy = -1;

        public ProxyInfo()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
            Ip = "";
            Name = "";
            Port = 0;
            UserName = "";
            Password = "";
            IsHttpConnect = false;
            SenderAddress = "";
            HostFqdm = "";
        }

        public static int Count { get { return proxies.Count; } }

        public string Id { get; }

        public string Ip { get; set; }

        public string Name { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsHttpConnect { get; set; }

        public string SenderAddress { get; set; }

        public string HostFqdm { get; set; }
    }
}