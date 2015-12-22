
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AsyncOAuth;
using Cadena._Internals;
using Cadena.Api;
using Cadena.Data;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena
{
    public sealed class ApiAccessor
    {
        public IOAuthCredential Credential { get; }

        public IRequestConfiguration RequestConfiguration { get; }

        public IProxyConfiguration ProxyConfiguration { get; }

        private readonly TwitterApiHttpClient _client;

        public ApiAccessor([NotNull] IOAuthCredential credential, [NotNull] IRequestConfiguration reqconf,
            [NotNull] IProxyConfiguration prxconf)
        {
            if (credential == null) throw new ArgumentNullException(nameof(credential));
            if (reqconf == null) throw new ArgumentNullException(nameof(reqconf));
            if (prxconf == null) throw new ArgumentNullException(nameof(prxconf));
            Credential = credential;
            RequestConfiguration = reqconf;
            ProxyConfiguration = prxconf;
            _client = new TwitterApiHttpClient(credential, reqconf, prxconf);
        }

        public async Task<IApiResult<T>> PostAsync<T>([NotNull] string path, [NotNull] HttpContent content,
            [NotNull] Func<HttpResponseMessage, Task<T>> converter, CancellationToken cancellationToken)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            using (var resp = await PostAsync(path, content, cancellationToken).ConfigureAwait(false))
            {
                return await converter(resp).ToApiResult(resp).ConfigureAwait(false);
            }
        }

        public async Task<IApiResult<T>> PostAsync<T>([NotNull] string path,
            [NotNull] IDictionary<string, object> parameter, [NotNull] Func<HttpResponseMessage, Task<T>> converter,
            CancellationToken cancellationToken)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            using (var resp = await PostAsync(path, parameter, cancellationToken).ConfigureAwait(false))
            {
                return await converter(resp).ToApiResult(resp).ConfigureAwait(false);
            }
        }

        public async Task<IApiResult<T>> GetAsync<T>([NotNull] string path,
            [NotNull] IDictionary<string, object> parameter, [NotNull] Func<HttpResponseMessage, Task<T>> converter,
            CancellationToken cancellationToken)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            using (var resp = await GetAsync(path, parameter, cancellationToken).ConfigureAwait(false))
            {
                return await converter(resp).ToApiResult(resp).ConfigureAwait(false);
            }
        }

        private Task<HttpResponseMessage> GetAsync([NotNull] string path,
            [NotNull] IDictionary<string, object> parameter,
            CancellationToken cancellationToken)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            var url = FormatUrl(RequestConfiguration.Endpoint, path, parameter.ParametalizeForGet());
            Debug.WriteLine("[GET] " + url);
            return _client.GetAsync(url, cancellationToken);
        }

        private Task<HttpResponseMessage> PostAsync([NotNull] string path,
            [NotNull] IDictionary<string, object> parameter, CancellationToken cancellationToken)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            return PostAsync(path, parameter.ParametalizeForPost(), cancellationToken);
        }

        private Task<HttpResponseMessage> PostAsync([NotNull] string path,
            [NotNull] HttpContent content, CancellationToken cancellationToken)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (content == null) throw new ArgumentNullException(nameof(content));
            var url = FormatUrl(RequestConfiguration.Endpoint, path);
            Debug.WriteLine("[POST] " + url);
            return _client.PostAsync(url, content, cancellationToken);
        }

        /// <summary>
        /// Spawn new HttpClient to connect the Stream API.
        /// </summary>
        /// <returns>spawned HttpClient</returns>
        internal HttpClient GetClientForStreaming()
        {
            return new TwitterApiHttpClient(Credential, new CompressionDisabledRequestConfiguration(RequestConfiguration), ProxyConfiguration);
        }

        private string FormatUrl([NotNull] string endpoint, [NotNull] string path)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (path == null) throw new ArgumentNullException(nameof(path));
            return HttpUtility.ConcatUrl(endpoint, path);
        }

        private string FormatUrl([NotNull] string endpoint, [NotNull] string path, [NotNull] string param)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (param == null) throw new ArgumentNullException(nameof(param));
            return String.IsNullOrEmpty(param)
                ? FormatUrl(endpoint, path)
                : FormatUrl(endpoint, path) + "?" + param;
        }

        /// <summary>
        /// HttpClient for accessing twitter.
        /// </summary>
        internal sealed class TwitterApiHttpClient : HttpClient
        {
            private const string UserAgentHeader = "User-Agent";

            public TwitterApiHttpClient(IOAuthCredential credential, IRequestConfiguration accessConfiguration,
                IProxyConfiguration proxyConfiguration)
                : base(GetInnerHandler(credential, accessConfiguration, proxyConfiguration))
            {
                DefaultRequestHeaders.Remove(UserAgentHeader);
                DefaultRequestHeaders.Add(UserAgentHeader, accessConfiguration.UserAgent);
            }

            private static HttpMessageHandler GetInnerHandler(IOAuthCredential credential,
                IRequestConfiguration reqconf,
                IProxyConfiguration prxconf)
            {
                var proxy = prxconf.UseSystemproxy
                    ? WebRequest.GetSystemWebProxy()
                    : prxconf.ProxyProvider?.Invoke();

                var exceptionHandler = new TwitterApiExceptionHandler(
                    new HttpClientHandler
                    {
                        AutomaticDecompression = reqconf.UseGZip
                            ? DecompressionMethods.GZip | DecompressionMethods.Deflate
                            : DecompressionMethods.None,
                        Proxy = proxy,
                        UseProxy = proxy != null
                    });

                return new OAuthMessageHandler(exceptionHandler,
                    credential.OAuthConsumerKey, credential.OAuthConsumerSecret,
                    new AccessToken(credential.OAuthAccessToken, credential.OAuthAccessTokenSecret));
            }
        }

        /// <summary>
        /// RequestConfiguration wrapper for disabling GZip
        /// </summary>
        private sealed class CompressionDisabledRequestConfiguration : IRequestConfiguration
        {
            private readonly IRequestConfiguration _baseConfiguration;

            public CompressionDisabledRequestConfiguration(IRequestConfiguration configuration)
            {
                _baseConfiguration = configuration;
            }

            public string Endpoint { get { return _baseConfiguration.Endpoint; } }
            public string UserAgent { get { return _baseConfiguration.UserAgent; } }
            public bool UseGZip { get { return false; } }
        }
    }
}
