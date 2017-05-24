// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//
// ! This file is a part of assets of AsyncOAuth project.
//
// AsyncOAuth - https://github.com/neuecc/AsyncOAuth from @neuecc
// This file is licensed under the MIT License. Check above link for more detail.
//
// Note: Several modifications may have been applied from original source by @ karno.
//
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AsyncOAuth
{
    // idea is based on http://blogs.msdn.com/b/henrikn/archive/2012/02/16/extending-httpclient-with-oauth-to-access-twitter.aspx
    public class OAuthMessageHandler : DelegatingHandler
    {
        string consumerKey;
        string consumerSecret;
        Token token;
        IEnumerable<KeyValuePair<string, string>> parameters;

        public OAuthMessageHandler(string consumerKey, string consumerSecret, Token token = null,
            IEnumerable<KeyValuePair<string, string>> optionalOAuthHeaderParameters = null)
            : this(new HttpClientHandler(), consumerKey, consumerSecret, token, optionalOAuthHeaderParameters)
        {
        }

        public OAuthMessageHandler(HttpMessageHandler innerHandler, string consumerKey, string consumerSecret,
            Token token = null, IEnumerable<KeyValuePair<string, string>> optionalOAuthHeaderParameters = null)
            : base(innerHandler)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.token = token;
            this.parameters = optionalOAuthHeaderParameters ?? Enumerable.Empty<KeyValuePair<string, string>>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            System.Threading.CancellationToken cancellationToken)
        {
            var encodedSendParams = parameters.Select(
                s => new KeyValuePair<string, string>(s.Key.UrlEncode(), s.Value.UrlEncode()));
            if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
            {
                // form url encoded content
                if (request.Content is FormUrlEncodedContent)
                {
                    // url encoded string
                    var extraParameter = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var parsed = Utility.SplitQueryString(extraParameter).ToArray();
                    request.Content = new PseudoFormUrlEncodedContent(parsed);

                    encodedSendParams = encodedSendParams.Concat(parsed); // preserve encoded
                }
            }

            var headerParams = OAuthUtility.BuildBasicParameters(
                consumerKey, consumerSecret,
                request.RequestUri.OriginalString, request.Method, token,
                encodedSendParams);
            headerParams = headerParams.Concat(parameters);

            var header = headerParams.Select(p => p.Key + "=" + p.Value.Wrap("\"")).ToString(",");
            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", header);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        sealed class PseudoFormUrlEncodedContent : ByteArrayContent
        {
            public PseudoFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> urlEncodedQueries)
                : base(GetContentByteArray(urlEncodedQueries))
            {
                Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            }

            private static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> urlEncodedQueries)
            {
                var param = String.Join("&", urlEncodedQueries.Select(pair => pair.Key + "=" + pair.Value));
                return Encoding.UTF8.GetBytes(param);
            }
        }
    }
}