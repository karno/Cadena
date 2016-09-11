using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cadena.OAuth._Internals;
using JetBrains.Annotations;

namespace Cadena.OAuth
{
    // idea is based on http://blogs.msdn.com/b/henrikn/archive/2012/02/16/extending-httpclient-with-oauth-to-access-twitter.aspx
    public class OAuthMessageHandler : DelegatingHandler
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly Token _token;
        private readonly IEnumerable<KeyValuePair<string, string>> _parameters;

        public OAuthMessageHandler([NotNull] string consumerKey, [NotNull] string consumerSecret,
            [CanBeNull] Token token = null,
            [CanBeNull] IEnumerable<KeyValuePair<string, string>> optionalOAuthHeaderParameters = null)
            : this(new HttpClientHandler(), consumerKey, consumerSecret, token, optionalOAuthHeaderParameters)
        {
        }

        public OAuthMessageHandler([NotNull] HttpMessageHandler innerHandler, [NotNull] string consumerKey,
            [NotNull] string consumerSecret, [CanBeNull] Token token = null,
            [CanBeNull] IEnumerable<KeyValuePair<string, string>> optionalOAuthHeaderParameters = null)
            : base(innerHandler)
        {
            if (innerHandler == null) throw new ArgumentNullException(nameof(innerHandler));
            if (consumerKey == null) throw new ArgumentNullException(nameof(consumerKey));
            if (consumerSecret == null) throw new ArgumentNullException(nameof(consumerSecret));
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _token = token;
            _parameters = optionalOAuthHeaderParameters ?? Enumerable.Empty<KeyValuePair<string, string>>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            [NotNull] HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var sendParameter = _parameters;
            if (request.Method == HttpMethod.Post)
            {
                // form url encoded content
                if (request.Content is FormUrlEncodedContent)
                {
                    // url encoded string
                    var extraParameter = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var parsed = Utility.ParseQueryString(extraParameter, true).ToArray(); // url decoded
                    sendParameter = sendParameter.Concat(parsed);

                    request.Content = new FormUrlEncodedContentEx(parsed);
                }
            }

            var headerParams = OAuthUtility.BuildBasicParameters(
                _consumerKey, _consumerSecret,
                request.RequestUri.OriginalString, request.Method, _token,
                sendParameter);
            headerParams = headerParams.Concat(_parameters);

            var header = headerParams.Select(p => p.Key + "=" + p.Value.Wrap("\"")).ToString(",");
            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", header);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        private sealed class FormUrlEncodedContentEx : ByteArrayContent
        {
            public FormUrlEncodedContentEx(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
                : base(GetContentByteArray(nameValueCollection))
            {
                Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            }

            private static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
            {
                StringBuilder stringBuilder = new StringBuilder();
                using (IEnumerator<KeyValuePair<string, string>> enumerator = nameValueCollection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<string, string> current = enumerator.Current;
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append('&');
                        }
                        stringBuilder.Append(Encode(current.Key));
                        stringBuilder.Append('=');
                        stringBuilder.Append(Encode(current.Value));
                    }
                }
                return Encoding.UTF8.GetBytes(stringBuilder.ToString());
            }

            private static string Encode(string data)
            {
                if (string.IsNullOrEmpty(data))
                {
                    return string.Empty;
                }

                return data.UrlEncode().Replace("%20", "+");
            }
        }
    }
}