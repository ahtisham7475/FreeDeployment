using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace swift.api.code.other
{
    public class ApiKey
    {
        public int Id { get; set; }

        public string  CreateTime { get; set; }
        public string APiKey { get; set; }
        public long InitCount { get; set; }
        public long ActualCount { get; set; }

    }
}