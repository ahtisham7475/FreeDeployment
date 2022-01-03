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

    // Utility class for cobisi library
    public class CobisiUtil
    {

        // Converts cobisi status to its string representation
        public static string StatusToString(CobisiStatus status)
        {
            switch (status)
            {
                default:
                case CobisiStatus.Unknown: return "Unknown";
                case CobisiStatus.Invalid: return "Invalid";
                case CobisiStatus.Valid: return "Valid";
            }
        }


        // Converts cobisi VerificationResult into CobisiStatus
        public static CobisiStatus GetStatus(VerificationResult result)
        {
            switch (result.ToString())
            {
                // Invalid Status Codes
                case "InvalidCharacterInSequence":
                case "InvalidWordBoundaryStar"://>>>>>>SyntaxError
                case "UnmatchedQuotedPair"://>>>>>>>> SyntaxError
                case "UnexpectedQuotedPairSequence"://>>>>>> SyntaxError
                case "UnbalancedCommentParenthesis"://>>>>>>> Syntax Error
                case "DoubleDotSequence"://>>>>>>>>> Syntax Error
                case "InvalidLocalPartLength"://>>>>>> Syntax Error
                case "InvalidFoldingWhiteSpaceSequence"://>>>>>> Syntax Error
                case "AtSignNotFound"://>>>>>>> Syntax Error
                case "InvalidEmptyQuotedWord"://>>>>>> Syntax Error
                case "InvalidAddressLength"://>>>>>>> Syntax Error
                case "DomainPartCompliancyFailure"://>>>>> Syntax Error
                case "DomainIsInexistent":
                case "DisposableEmailAddress":
                case "MailboxDoesNotExist":
                case "HttpConnectionFailure":
                    return CobisiStatus.Invalid;

                // Valid Status Codes
                case "Success":
                case "CatchAllValidationTimeout":
                case "CatchAllConnectionFailure":
                    return CobisiStatus.Valid;

                // Unknown Status Codes
                default:
                case "DnsQueryTimeout":
                case "DnsConnectionFailure":
                case "ServerIsCatchAll":
                case "SmtpConnectionTimeout":
                case "MailboxConnectionFailure":
                case "UnhandledException":
                case "MailboxValidationTimeout":
                case "ServerDoesNotSupportInternationalMailboxes":
                case "MailboxTemporarilyUnavailable":
                case "LocalSenderAddressRejected":
                    return CobisiStatus.Unknown;

            }
        }
    }
}