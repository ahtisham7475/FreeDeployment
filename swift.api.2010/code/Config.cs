using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace swift.api.code
{
    public static class Config
    {

        /*
         * This property will set up how much time to wait for a task to complete 
         * its work. If this value is reached, an timeout exeprion will be raise and the
         * task will be cancelled. In milliseconds
         */
        public static readonly int TASKS_TIMEOUT = 60 * 1000 * int.Parse(ConfigurationManager.AppSettings["TASKS_TIMEOUT"]); //2; // MILLISECONDS!

        //static int f =  int.Parse(ConfigurationManager.AppSettings["ContentUrl"])

        /*
         * This property sets how meny proxies to be used per verification task.
         * Use ProxiInfo.Count to find how many different proxy configurations are available.
         */
        public static readonly int NUMBER_OF_PROXIES = int.Parse(ConfigurationManager.AppSettings["NUMBER_OF_PROXIES"]);// 5;



        /*
         * This property let you specifiy if you want to allocate proxy configurations
         * to be used randomly. The default value is false, which means that proxy configurations
         * will be allocated based on thei order into collection.
         */
        public static readonly bool ALLOCATE_PROXY_RANDOMLY = bool.Parse(ConfigurationManager.AppSettings["ALLOCATE_PROXY_RANDOMLY"]);// true  ;



        /*
         * Verifications that employ level or greater and that are made for the Smpt same email domain may overload
         *  external mail exchangers with excessive requests in a small amount of time. To respect Netiquette and to prevent
         *  their banning the IP address used to place the verification requests, it is possible to specify an interval expressed in
         *  seconds that the component will observe between subsequent connections to the same SMTP server.
         */
        public static readonly TimeSpan SAME_DOMAIN_CONNECTION_DELAY = new TimeSpan(0, 0, int.Parse(ConfigurationManager.AppSettings["SAME_DOMAIN_CONNECTION_DELAY"])); // 10); // 10 SECONDS!



        /*
         * This setting allows you to limit the total number of seconds to wait for a complete mailbox check. Tasks lasting
         *   more than this timeout are automatically aborted and yield to the failure of the verification. In seconds.
         */
        public static readonly TimeSpan MAILBOX_INSPECTION_TIMEOUT = new TimeSpan(0, 0, int.Parse(ConfigurationManager.AppSettings["MAILBOX_INSPECTION_TIMEOUT"]));// 120); // 30 SECONDS!  DEFAULT VALUE: 60 SECONDS



        /*
         * This setting allows you to limit the total number of seconds to wait for mail exchangers to send a response to the
         *  component's first enquiry. Connections lasting more than this timeout are automatically aborted and yield to the
         *  failure of the verification. In seconds.
         */
        public static readonly TimeSpan SMTP_CONNECTION_TIMEOUT = new TimeSpan(0, 0, int.Parse(ConfigurationManager.AppSettings["SMTP_CONNECTION_TIMEOUT"]));//); // 10 SECONDS!



        /*
         * This setting allows you to adjust the behavior of the internal DNS resolver of the component, limiting the total
         *  number of seconds to wait for each DNS query to complete. Queries lasting more than this timeout are
         *  automatically aborted and yield to the failure of the verification. In seconds.
         */
        public static readonly TimeSpan DNS_QUERY_TIMEOUT = new TimeSpan(0, 0, int.Parse(ConfigurationManager.AppSettings["DNS_QUERY_TIMEOUT"]));/// 120); // 10 SECONDS!


        /*
         * Altering this setting changes the number of times you wish to retry querying the DNS server(s) when a connection
         *   failure or a timeout occurs.
         */
        public static readonly int DNS_MAX_RETRIES = int.Parse(ConfigurationManager.AppSettings["DNS_MAX_RETRIES"]);// 18;



        /**
         * If quoted strings are allowed, the component will correctly validate email addresses that contain labels within
         *  quotes and special characters included in quoted pairs. It is highly recommended that you leave this feature on, as
         *  many syntactically valid addresses are built in such a way. 
         *   
         *  For example:
         *  john. "is \@ home "@example.com
         *  a."..".b@example.org
         *  "hi there"@example.net
         */
        public static readonly bool ALLOW_QUOTED_STRINGS = bool.Parse(ConfigurationManager.AppSettings["ALLOW_QUOTED_STRINGS"]);// true;


        /*
         * If comments are allowed, the component will support the email addresses that contain comments either in the
         *   local or in the domain parts. Even if it is rare nowadays to see such usage, comments are still syntactically valid,
         *   and you may want to leave this feature on whenever you are validating addresses coming from legacy systems. 
         *   
         *   For example:
         *   michael(this is my mailbox)@example.com
         *   john(local part)@(domain part)example.com(tld)
         */
        public static readonly bool ALLOW_COMMENTS = bool.Parse(ConfigurationManager.AppSettings["ALLOW_COMMENTS"]);// true;


        /*
         * Folding White Spaces (FWS) are particular combinations of <CR>, <LF>, and <SP> characters used by older mail
         *  exchangers to split long email addresses. If this setting is turned on, the component will correctly validate
         *  addresses that contain FWS. 
         *  
         *  For example:
         *  john<CR><LF><SP>@example.org
         */
        public static readonly bool ALLOW_FOLDING_SPACES = bool.Parse(ConfigurationManager.AppSettings["ALLOW_FOLDING_SPACES"]);// true;


        /*
         * If domain literals are allowed, the component will support email addresses whose domain parts are IPv4 or IPv6
         *  addresses, expressed according to IETF standards. 
         *  
         *  For example:
         *  bill@[127.0.0.1]
         *  john.smith@[IPv6:::12.34.56.78]
         */
        public static readonly bool ALLOW_DOMAIN_LITERALS = bool.Parse(ConfigurationManager.AppSettings["ALLOW_DOMAIN_LITERALS"]);// true;


        /*
         * If Internationalized Domain Names (IDN) are allowed, the component accepts email addresses whose domain parts
         *  are encoded with an encoding different from US-ASCII. Same for allowing international mailboxes apply.
         *  
         *  For example:
         *  george@bücher.ch
         *  postmaster@ مصر.الأت صالا ت-وزارة
         */
        public static readonly bool ALLOW_INTERNATIONAL_DOMAIN_NAMES = bool.Parse(ConfigurationManager.AppSettings["ALLOW_INTERNATIONAL_DOMAIN_NAMES"]);// true;
        public static readonly bool ALLOW_INTERNATIONAL_MAILBOX_NAMES = bool.Parse(ConfigurationManager.AppSettings["ALLOW_INTERNATIONAL_MAILBOX_NAMES"]);// true;
    }
}