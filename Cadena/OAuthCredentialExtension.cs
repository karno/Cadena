using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Cadena.Api;
using AsyncOAuth;

namespace Cadena
{
    public static class OAuthCredentialExtension
    {
        private const string UserAgentHeader = "User-Agent";


        public static HttpClient CreateOAuthClient(
            this IApiAccess access,
            IEnumerable<KeyValuePair<string, string>> optionalHeaders = null,
            bool useGZip = true)
        {
            var credential = access.Credential;
            return new HttpClient(
                new OAuthMessageHandler(
                    GetInnerHandler(access, useGZip),
                    credential.OAuthConsumerKey, credential.OAuthConsumerSecret,
                    new AccessToken(credential.OAuthAccessToken, credential.OAuthAccessTokenSecret),
                    optionalHeaders), true);
        }

        public static HttpClient SetUserAgent(this HttpClient client, string userAgent)
        {
            // remove before add user agent
            client.DefaultRequestHeaders.Remove(UserAgentHeader);
            client.DefaultRequestHeaders.Add(UserAgentHeader, userAgent);
            return client;
        }

        private static HttpMessageHandler GetInnerHandler(IApiAccess access, bool useGZip)
        {
            var proxy =
                access.ProxyConfiguration.UseSystemproxy
                    ? WebRequest.GetSystemWebProxy()
                    : (access.ProxyConfiguration.ProxyProvider == null ? null : access.ProxyConfiguration.ProxyProvider());
            return new TwitterApiExceptionHandler(
                new HttpClientHandler
                {
                    AutomaticDecompression = useGZip
                        ? DecompressionMethods.GZip | DecompressionMethods.Deflate
                        : DecompressionMethods.None,
                    Proxy = proxy,
                    UseProxy = proxy != null
                });
        }

    }
}
