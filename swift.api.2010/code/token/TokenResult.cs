using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace swift.api.code.token
{
    public class TokenResult
    {
        public string Token { get; set; }

        public long InitalQueryCount { get; set; }

        public long ActualQueryCount { get; set; }

        public string Status { get; set; }
    }
}