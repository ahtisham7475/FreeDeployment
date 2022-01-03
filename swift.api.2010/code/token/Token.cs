using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

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

namespace swift.api.code.token
{

    // This class is responsible for managing tokens
    public class Token
    {
        // token versions collection
        private static Dictionary<string, double> versions;

        // Token version which is currently used
        private static readonly string CURRENT_VERSION = "4AFBAA3B-79CE-4C01-839F-206DB1AE9D5C";

        // keys collection
        private static readonly List<string> keys;


        private string _id;

        private string _version;

        private string _key;

        private long _initQuota;

        private long _currentQuota;

        private string _requestSource;

        private DateTime _creationDate;


        private Token()
        {
            // used only internally to construct an token from a string
        }


        // class public constructor
        public Token(string key, long quota, long current)
        {
            if (String.IsNullOrEmpty(key) || String.IsNullOrWhiteSpace(key))
                throw new Exception("Key is required when creating a token.");

            _id = Guid.NewGuid().ToString().Replace("-", "");
            _version = CURRENT_VERSION;
            _key = key;
            _initQuota = (quota > 0) ? quota : 0;
            _currentQuota = (current > 0) ? current : 0;
            _requestSource = GetRemoteAddress();
            _creationDate = DateTime.Now;
        }


        public string Id { get { return _id; } }

        public string Key { get { return _key; } }

        public long InitialQuota { get { return _initQuota; } }


        public long CurrentQuota
        {
            get { return _currentQuota; }
            set
            {
                if (value <= _currentQuota)
                    _currentQuota = (value < 0) ? 0 : value;
            }
        }



        // rearms the current token
        public void Rearm()
        {
            if (!IsExpired())
            {
                _creationDate = DateTime.Now;
                string ip = GetRemoteAddress();
                if (ip != _requestSource && !String.IsNullOrEmpty(ip))
                    _requestSource = ip;
            }
        }



        // check if a token belongs to a specific user
        public bool BelongsTo(string apiKey)
        {
            if (String.IsNullOrEmpty(apiKey) || String.IsNullOrWhiteSpace(apiKey))
                return false;

            if (apiKey == _key)
                return true;
            return false;
        }



        // check if the current token is expired
        public bool IsExpired()
        {
            DateTime now = DateTime.Now;
            TimeSpan dif = now - _creationDate;
            if (dif.Days > 0)
                return true;
            if (dif.Hours > 0)
                return true;
            if (dif.Minutes > versions[_version])
                return true;
            return false;
        }



        // gets the string token as base64 encoded string
        override public string ToString() { return Encrypt(); }


        // gets the string token as rl encoded string
        public string ToUrlEncoded() { return HttpServerUtility.UrlTokenEncode(Convert.FromBase64String(Encrypt())); }



        // gets the user remote address
        private string GetRemoteAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;

            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                    return addresses[0];
            }
            return context.Request.ServerVariables["REMOTE_ADDR"];
        }



        // encrypts current token
        private string Encrypt()
        {
            // init
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            aes.GenerateIV();

            Random rnd = new Random();
            string key = keys[rnd.Next(0, keys.Count)];
            aes.Key = Convert.FromBase64String(key);
            string iv = Convert.ToBase64String(aes.IV).TrimEnd('=');

            // get token
            string json = ToJson();
            byte[] plainText = ASCIIEncoding.UTF8.GetBytes(json);

            ICryptoTransform crypto = aes.CreateEncryptor();
            byte[] cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);

            return iv + Convert.ToBase64String(cipherText);
        }



        // tries to decrypt a string and to obtain a token
        private static bool Decrypt(string item, string key, out string result)
        {
            try
            {
                string iv = item.Substring(0, 22) + "==";
                string token = item.Substring(22);

                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.IV = Convert.FromBase64String(iv);
                aes.Key = Convert.FromBase64String(key);

                byte[] cipherToken = Convert.FromBase64String(token);

                ICryptoTransform decrypto = aes.CreateDecryptor();

                byte[] decryptedText = decrypto.TransformFinalBlock(cipherToken, 0, cipherToken.Length);
                string json = ASCIIEncoding.UTF8.GetString(decryptedText);
                result = json;
                return true;
            }

            catch (Exception) { result = null; return false; }
        }



        // returns token as a json string
        private string ToJson()
        {
            string json = "{";
            json += "i:\"" + _id + "\",";
            json += "v:\"" + _version + "\",";
            json += "k:\"" + _key + "\",";
            json += "q:" + _initQuota + ",";
            json += "c:" + _currentQuota + ",";
            json += "s:\"" + _requestSource + "\",";

            json += "d:" + _creationDate.Day + ",";
            json += "m:" + _creationDate.Month + ",";
            json += "u:" + _creationDate.Year + ",";
            json += "x:" + _creationDate.Hour + ",";
            json += "y:" + _creationDate.Minute + ",";
            json += "z:" + _creationDate.Second + "}";

            return json;
        }



        // checks if an string is token or not
        public static bool IsToken(string item)
        {
            bool success = false;
            string result = null;
            for (int i = 0; i < keys.Count; i++)
            {
                success = Decrypt(item, keys[i], out result);
                if (success) break;
            }
            return success;
        }


        // gets the token from url encoded string
        public static Token FromUrlEncodedString(string token)
        {
            try
            {
                string item = Convert.ToBase64String(HttpServerUtility.UrlTokenDecode(token));
                return FromString(item);
            }
            catch { return null; }
        }


        // gets the token from base64 string
        public static Token FromString(string token)
        {
            Token tkn = null;

            bool success = false;
            string result = null;
            for (int i = 0; i < keys.Count; i++)
            {
                success = Decrypt(token, keys[i], out result);
                if (success) break;
            }

            if (success)
            {
                try
                {
                    var serializer = new JavaScriptSerializer();
                    Dictionary<string, object> g = (Dictionary<string, object>)serializer.DeserializeObject(result);

                    if (!(g.ContainsKey("i") ||
                        g.ContainsKey("v") ||
                        g.ContainsKey("k") ||
                        g.ContainsKey("q") ||
                        g.ContainsKey("c") ||
                        g.ContainsKey("s") ||
                        g.ContainsKey("d") ||
                        g.ContainsKey("m") ||
                        g.ContainsKey("u") ||
                        g.ContainsKey("x") ||
                        g.ContainsKey("y") ||
                        g.ContainsKey("z")))
                        return null;

                    tkn = new Token();
                    tkn._id = (string)g["i"];
                    tkn._version = (string)g["v"];
                    tkn._key = (string)g["k"];
                    tkn._initQuota = (int)g["q"];
                    tkn._currentQuota = (int)g["c"];
                    tkn._requestSource = (string)g["s"];
                    tkn._creationDate = new DateTime((int)g["u"], (int)g["m"], (int)g["d"], (int)g["x"], (int)g["y"], (int)g["x"]);

                    if (!versions.ContainsKey(tkn._version))
                        return null;

                    if (tkn.IsExpired())
                        return null;

                    if (tkn._requestSource != tkn.GetRemoteAddress())
                        return null;
                }

                catch (Exception) { return null; }
            }

            return tkn;
        }



        // static constructor
        static Token()
        {

            // This will keep various valid token versions together with their valid time interval
            versions = new Dictionary<string, double>();

            // Be careful that versions to be unique !
            //  Removing a version will invalidate all tokens with its version being the same with the removed one
            versions.Add("4AFBAA3B-79CE-4C01-839F-206DB1AE9D5C", 30); // Version id - Validation time (MINUTES)


            // set keys
            keys = new List<string>();
            keys.Add("dTi5nSbfDfVYnJr1CzofpUl9HHsEzAljzA6avB+Uvvk=");
            keys.Add("o4SoLuGLTyzMzNGWVDqM2Eqe3pPsk5EtHh/njPWypvk=");
            keys.Add("0pB3Jzk1SpgBgadEJCUEpNt8WKDQs0h3XteHKHGElw0=");
            keys.Add("FBrtewlHtwsBns1tgd/wU62ih2dHIE/QETYMII5YHfA=");
            keys.Add("VCGsYaKkwMTpQdp7z65SMGwEbGZ3Pko4o6U9CJYphHc=");
        }

    }
}