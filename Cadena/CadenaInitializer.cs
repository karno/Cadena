using System;
using System.Net;
using System.Security.Cryptography;
using AsyncOAuth;

namespace Cadena
{
    public static class CadenaInitializer
    {
        public static void Initialize()
        {
            // set ServicePointManager properties.
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = Int32.MaxValue;
            OAuthUtility.ComputeHash = (key, buffer) => { using (var hmac = new HMACSHA1(key)) { return hmac.ComputeHash(buffer); } };
        }
    }
}
