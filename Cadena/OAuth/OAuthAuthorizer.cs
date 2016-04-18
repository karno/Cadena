/************************************************************
The MIT License (MIT)
Copyright 2013-2014 neuecc, AsyncOAuth project
Copyright 2016 Karno

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cadena.OAuth._Internals;
using JetBrains.Annotations;

namespace Cadena.OAuth
{
    /// <summary>OAuth Authorization Client</summary>
    public class OAuthAuthorizer
    {
        readonly string _consumerKey;
        readonly string _consumerSecret;

        public OAuthAuthorizer(string consumerKey, string consumerSecret)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
        }

        private async Task<TokenResponse<T>> GetTokenResponse<T>([NotNull] string url,
            [NotNull] OAuthMessageHandler handler, [CanBeNull] HttpContent postValue,
            [NotNull] Func<string, string, T> tokenFactory) where T : Token
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (tokenFactory == null) throw new ArgumentNullException(nameof(tokenFactory));

            var client = new HttpClient(handler);

            var response = await client.PostAsync(url,
                        postValue ?? new FormUrlEncodedContent(Enumerable.Empty<KeyValuePair<string, string>>()))
                          .ConfigureAwait(false);
            var tokenBase = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException(response.StatusCode + ":" + tokenBase); // error message
            }

            var splitted = tokenBase.Split('&').Select(s => s.Split('=')).ToLookup(xs => xs[0], xs => xs[1]);
            var token = tokenFactory(splitted["oauth_token"].First().UrlDecode(),
                splitted["oauth_token_secret"].First().UrlDecode());
            var extraData = splitted.Where(kvp => kvp.Key != "oauth_token" && kvp.Key != "oauth_token_secret")
                                    .SelectMany(g => g, (g, value) => new { g.Key, Value = value })
                                    .ToLookup(kvp => kvp.Key, kvp => kvp.Value);
            return new TokenResponse<T>(token, extraData);
        }

        /// <summary>construct AuthrizeUrl + RequestTokenKey</summary>
        public string BuildAuthorizeUrl([NotNull] string authUrl, [NotNull] RequestToken requestToken)
        {
            if (authUrl == null) throw new ArgumentNullException(nameof(authUrl));
            if (requestToken == null) throw new ArgumentNullException(nameof(requestToken));

            return authUrl + "?oauth_token=" + requestToken.Key;
        }

        /// <summary>asynchronus get RequestToken</summary>
        public Task<TokenResponse<RequestToken>> GetRequestToken([NotNull] string requestTokenUrl,
            [CanBeNull] IEnumerable<KeyValuePair<string, string>> parameters = null,
            [CanBeNull] HttpContent postValue = null)
        {
            if (requestTokenUrl == null) throw new ArgumentNullException(nameof(requestTokenUrl));

            var handler = new OAuthMessageHandler(_consumerKey, _consumerSecret, null, parameters);
            return GetTokenResponse(requestTokenUrl, handler, postValue, (key, secret) => new RequestToken(key, secret));
        }

        /// <summary>asynchronus get GetAccessToken</summary>
        public Task<TokenResponse<AccessToken>> GetAccessToken([NotNull] string accessTokenUrl,
            [NotNull] RequestToken requestToken, [NotNull] string verifier,
            [CanBeNull] IEnumerable<KeyValuePair<string, string>> parameters = null,
            [CanBeNull] HttpContent postValue = null)
        {
            if (accessTokenUrl == null) throw new ArgumentNullException(nameof(accessTokenUrl));
            if (requestToken == null) throw new ArgumentNullException(nameof(requestToken));
            if (verifier == null) throw new ArgumentNullException(nameof(verifier));

            var verifierParam = new KeyValuePair<string, string>("oauth_verifier", verifier.Trim());

            if (parameters == null) parameters = Enumerable.Empty<KeyValuePair<string, string>>();
            var handler = new OAuthMessageHandler(_consumerKey, _consumerSecret, requestToken,
                parameters.Concat(new[] { verifierParam }));

            return GetTokenResponse(accessTokenUrl, handler, postValue, (key, secret) => new AccessToken(key, secret));
        }
    }
}