using MySql.Data.MySqlClient;
using swift.api.code.token;
using System;
using System.Collections.Concurrent;
using System.Threading;


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
    // This class is responsible for api key management
    public class ApiKeyManager
    {        
        static readonly bool UseMySQL = true;

        static readonly int ConnectionTimeout = 15;
        // connection string (private and immutable/HAPROXY Loadd Balancer)
        private static readonly string DB_STRING = "Server=95.216.10.237;Port=3306;Database=bpsocks;User ID=jarvis;Password=CrypticP001!;Connect Timeout=" + ConnectionTimeout + ";";

        // Individual DB (for testing)
        //private static readonly string DB_STRING = "Server=13.67.237.162;Port=3306;Database=bpsocks;User ID=bpsocksapp;Password=appasp.net787912;";

        // check if the api key is valid
        public static readonly string CHECK_API = "SELECT InitCount, ActualCount FROM mailchecker_apikeys WHERE ApiKey = @apiKey;";

        // update user queries quota
        public static readonly string SET_QUOTA = "UPDATE mailchecker_apikeys SET ActualCount = ActualCount - 1 WHERE ApiKey = @apikey;";

        private readonly string _id;

        public ApiKeyManager()
        {
            _id = Guid.NewGuid().ToString().Replace("-", "");
        }
        
        // authenticates based on api key and resturns an token to be used 
        //  with subsequent requests. It also returns the sql connection for reusing it.
        public Token Authenticate(string key, out bool success, ConcurrentDictionary<string, ConcurrentBag<string>> statusCodes)
        {
            Token response = null;
            MySqlConnection conn = new MySqlConnection(DB_STRING);
            success = false;

            //create & intialize key object
            ApiKey objKey = new ApiKey() { Id = -1, ActualCount = 0, APiKey = key, CreateTime = "", InitCount = 0 };

            if (UseMySQL)
            {
                try
                {
                    conn.Open();

                    MySqlCommand command = conn.CreateCommand();
                    command.Parameters.Add(new MySqlParameter("@apikey", key));
                    command.CommandText = CHECK_API;
                    command.CommandTimeout = ConnectionTimeout;

                    using (var Reader = command.ExecuteReader())
                    {
                        if (Reader.Read())
                        {
                            objKey.InitCount = (int)Reader.GetValue(0);
                            objKey.ActualCount = (int)Reader.GetValue(1);
                            success = true;
                        }
                    }


                }

                catch (Exception ex)
                {

                    statusCodes["Error"].Add(StatusCode.DATABASE_ERROR + " => " + ex.Message + " => " + ex.StackTrace);
                    success = false;
                }

                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            if (objKey.InitCount < 0)
            {
                statusCodes["Unknown"].Add(StatusCode.INVALID_TOKEN);
                response = null;    // authentication failed!
            }
            else
            {
                response = new Token(key, objKey.InitCount, objKey.ActualCount);
            }

            return response;
        }        

        // updates queries quota for specific user
        public void UpdateQuota(Token token)
        {
            if (UseMySQL)
            {
                MySqlConnection conn = new MySqlConnection(DB_STRING);
                try
                {
                    conn.Open();

                    MySqlCommand command = conn.CreateCommand();
                    command.Parameters.Add(new MySqlParameter("@apikey", token.Key));
                    command.CommandText = SET_QUOTA;
                    int res = command.ExecuteNonQuery();
                }

                catch (Exception) { }

                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
    }
}