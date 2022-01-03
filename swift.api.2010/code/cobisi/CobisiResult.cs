using Cobisi.EmailVerify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * AUTHOR: Stefan Kogut
 * Created: 2015.
 * *
 * Hire me: https://www.freelancer.com/u/stefankogut.html
 * Email: stefan.kogut@yahoo.com, sk.stefankogut@gmail.com
 * Copyright 2015 Stefan Kogut
 * All Rights Reserved.
 * *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 * *
 * *** Redistributions of source code must retain the above copyright notice,
 * this list of conditions and the following disclaimer.
 * *
 * *** Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in the
 * documentation and/or other materials provided with the distribution.
 * *
 * *** Neither the name of Stefan Kogut nor the names of
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 * *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
 * IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace swift.api.code.cobisi
{
    // This class id responsible for publishing the 
    //  verification result from cobisi library
    public class CobisiResult
    {

        private string _id;

        private string _email;

        private VerificationLevel _level;

        private VerificationResult _result;

        private List<string> _log;

        public CobisiResult()
        {
            _id = Guid.NewGuid().ToString().Replace("-", "");
            _log = new List<string>();
        }


        public string Id { get { return _id; } }

        public VerificationResult Result { get { return _result; } set { _result = value; } }

        public string EmailAddress { get { return _email; } set { _email = value; } }

        public VerificationLevel Level { get { return _level; } set { _level = value; } }

        public List<string> Log { get { return _log; } set { _log = value; } }
    }
}