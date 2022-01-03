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

namespace swift.api.code.other
{
    // This class is responsible for managing output status codes
    public class StatusCode
    {

        // Valid Status Codes


        /*
         * The email was successfully verified as Valid.
         */
        public static readonly string MAILBOX_EXISTS_AND_ACTIVE = "Mailbox Exists and Active"; 



        // Invalid Status Codes


        /*
         * This failure means that the email address is provided by a well-known 
         * disposable email address provider (DEA) such as mailinator.com.
         */
        public static readonly string KNOWN_DISPOSABLE_EMAIL_DOMAIN = "Known Disposable Email Domain"; 


        /*
         * This failure means that the email is not syntactically correct.
         */
        public static readonly string SYNTAX_ERROR = "Syntax Error";


        /*
         * This means that the email domain has be found to be non-existent.
         */
        public static readonly string DOMAIN_DOES_NOT_EXIST = "Domain Does Not Exist";


        /*
         * This failure means that the mailbox for the provided email address does not exist.
         */
        public static readonly string MAILBOX_NOT_FOUND = "Mailbox Not Found";


        /*
         * This status code indicates that the email address contained a curse word which 
         * most probably indicate it is a fake email address. E.g: fuck@yahoo.com.
         */
        public static readonly string CURSE_WORD_CHECK = "Curse Words Check";


        /*
         * This status code indicates that the email address was detected to be fake 
         * using the API in-built fake email pattern detection algorithm. 
         * E.g: ususjsusjsjsjjss@yahoo.com.
         */
        public static readonly string FAKE_EMAIL_PATTERN_MATCH = "Fake Email Pattern Match";


        /*
         * This status code indicates that a typo error was 
         * detected for a known email domain such as : john@hotmaill.com.
         */
        public static readonly string TYPO_CHECKING = "Typo Checking";



        // Unknown Statuse Codes


        /*
         * An invalid API key was used. Please check the API key and make sure it is correct.
         */
        public static readonly string INVALID_TOKEN = "InvalidToken";   


        /*
         * There was an unexpected error on our server.
         */
        public static readonly string INTERNAL_ERROR = "InternalError";


        /*
         * This indicates that there was a database connection error from our API server
         */
        public static readonly string DATABASE_ERROR = "InternalDBError";


        /*
         * The allocated # of queries or requests for the API key has been exhausted.
         */
        public static readonly string NO_MORE_QUERIES = "NoMoreQueries";


        /*
         * This failure means that there was a DNS error when querying the MX server
         */
        public static readonly string DNS_QUERY_ERROE = "DNS Query Error";


        /*
         * This failure means that the external mail exchanger rejected the 
         * local sender address or the incoming connecting IP.
         */
        public static readonly string SMTP_CONNECTION_BLOCKED = "SMTP Connection Blocked";


        /*
         * This failure means that a timeout or error occurred while verifying 
         * the existence of the mailbox for the provided email address.
         */
        public static readonly string MAILBOX_VALIDATION_ERROR = "Mailbox Validation Error";


        /*
         * This failure means that the requested mailbox is temporarily unavailable; this is not an 
         * indicator that the mailbox actually exists or not but, often, a message sent by external 
         * mail exchangers with greylisting enabled.
         */
        public static readonly string MAILBOX_TEMPORARY_NOT_REACHABLE = "Mailbox temporary not reachable (Graylisting)";


        /*
         * This failure means that the email address could be verified 
         * because the remote server was not responding.
         */
        public static readonly string MAILBOX_NOT_REACHABLE = "Mailbox Not Reachable";


        /*
         * This failure means that the external mail exchanger under test accepts fake, 
         * non existent, email addresses; therefore the provided email address MAY be inexistent too. 
         * In most cases, these Catch-all domains are now setup by ISPs and ESPs as 
         * Catch-all Spam Trap domains specifically targeted to catch 
         * spammers using Dictionary Spam Attacks.
         */
        public static readonly string CATCH_ALL_EMAIL_DOMAIN = "Catchall Email Domain";


        /*
         * This failure means that a connection could not be established with the remote SMTP server.
         */
        public static readonly string SMTP_CONNECTION_ERROR = "SMTP Connection Error";


        // gets the status code
        public static string GetOutputStatusCode(string code)
        {
            string result = code;
            switch (code)
            {
                // Invalid Status Codes
                case "FailedBasicSyntax":   // from kellerman
                case "InvalidCharacterInSequence":
                case "InvalidWordBoundaryStar":
                case "UnmatchedQuotedPair":
                case "UnexpectedQuotedPairSequence":
                case "UnbalancedCommentParenthesis":
                case "DoubleDotSequence":
                case "InvalidLocalPartLength":
                case "InvalidFoldingWhiteSpaceSequence":
                case "AtSignNotFound":
                case "InvalidEmptyQuotedWord":
                case "InvalidAddressLength":
                case "DomainPartCompliancyFailure": result = SYNTAX_ERROR; break;

                case "DomainIsInexistent": result = DOMAIN_DOES_NOT_EXIST; break;
                case "DisposableEmailAddress": result = KNOWN_DISPOSABLE_EMAIL_DOMAIN; break;
                case "MailboxDoesNotExist": result = MAILBOX_NOT_FOUND; break;
                case "ServerIsCatchAll": result = CATCH_ALL_EMAIL_DOMAIN; break;
                case "HttpConnectionFailure": result = MAILBOX_NOT_FOUND; break;
                
                // From Kellerman
                case "FailedCurseWords": result = CURSE_WORD_CHECK; break;
                case "FailedTypo": result = TYPO_CHECKING; break;
                case "FailedFakeEmailPatternMatcher": result = FAKE_EMAIL_PATTERN_MATCH; break;


                // Valid Status Codes
                case "PassedAllRequestedValidation":  // from kellerman
                case "Success":
                case "CatchAllValidationTimeout":
                case "CatchAllConnectionFailure": result = MAILBOX_EXISTS_AND_ACTIVE; break;

                // Unknown Status Codes
                case "DnsQueryTimeout": result = DNS_QUERY_ERROE; break;
                case "DnsConnectionFailure": result = DNS_QUERY_ERROE; break;
                case "SmtpConnectionTimeout": result = SMTP_CONNECTION_ERROR; break;
                case "MailboxConnectionFailure": result = MAILBOX_NOT_REACHABLE; break;
                case "UnhandledException": result = MAILBOX_VALIDATION_ERROR; break;
                case "MailboxValidationTimeout": result = MAILBOX_VALIDATION_ERROR; break;
                case "ServerDoesNotSupportInternationalMailboxes": result = MAILBOX_VALIDATION_ERROR; break;
                case "MailboxTemporarilyUnavailable": result = MAILBOX_TEMPORARY_NOT_REACHABLE; break;
                case "LocalSenderAddressRejected": result = SMTP_CONNECTION_BLOCKED; break;

            }
            return result;
        }


    }
}