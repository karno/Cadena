
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    public sealed class ApiAccessor : IApiAccessor
    {
        #region Provide default configuration

        public const string DefaultEndpoint = "https://api.twitter.com/1.1/";

        public const string DefaultEndpointForUpload = "https://upload.twitter.com/1.1/";

        public const string DefaultEndpointForUserStreams = "https://userstream.twitter.com/1.1/";

        public const string DefaultUserAgent = "Project Cadena/Alchemic Library for Twitter API.";

        /// <summary>
        /// System proxy, equals to WebRequest.GetSystemWebProxy
        /// </summary>
        /// <returns>System web proxy</returns>
        public static IWebProxy GetSystemWebProxy()
        {
            return WebRequest.DefaultWebProxy;
        }

        #endregion

        public long Id { get { return Credential.Id; } }

        /// <summary>
        /// Credential information of this accessor.
        /// </summary>
        public IOAuthCredential Credential { get; }

        public string Endpoint { get; }

        public string UserAgent { get; }

        public IWebProxy Proxy { get; }

        private readonly Lazy<HttpClient> _client;

        private bool _disposed;

        public ApiAccessor([NotNull] IOAuthCredential credential, [NotNull] string endpoint,
            [CanBeNull] IWebProxy proxy, string userAgent = null, bool useGzip = true)
        {
            if (credential == null) throw new ArgumentNullException(nameof(credential));
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            Credential = credential;
            Endpoint = endpoint;
            Proxy = proxy;
            UserAgent = userAgent ?? DefaultUserAgent;
            _client = new Lazy<HttpClient>(() => new TwitterApiHttpClient(credential, proxy, UserAgent, useGzip),
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public async Task<IApiResult<T>> PostAsync<T>(string path, HttpContent content,
            Func<HttpResponseMessage, Task<T>> converter, CancellationToken cancellationToken)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            using (var resp = await PostAsync(path, content, cancellationToken).ConfigureAwait(false))
            {
                return await converter(resp).ToApiResult(resp).ConfigureAwait(false);
            }
        }

        public async Task<IApiResult<T>> PostAsync<T>(string path,
            IDictionary<string, object> parameter, Func<HttpResponseMessage, Task<T>> converter,
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

        public async Task<IApiResult<T>> GetAsync<T>(string path,
            IDictionary<string, object> parameter, Func<HttpResponseMessage, Task<T>> converter,
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

            var url = FormatUrl(Endpoint, path, parameter.ParametalizeForGet());
            Debug.WriteLine("[GET] " + url);
            return _client.Value.GetAsync(url, cancellationToken);
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
            var url = FormatUrl(Endpoint, path);
            Debug.WriteLine("[POST] " + url);
            return _client.Value.PostAsync(url, content, cancellationToken);
        }

        public async Task ConnectStreamAsync(string path, IDictionary<string, object> parameter,
            Func<Stream, Task> streamReader, CancellationToken cancellationToken)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (streamReader == null) throw new ArgumentNullException(nameof(streamReader));
            HttpClient client = null;
            try
            {
                var endpoint = HttpUtility.ConcatUrl(Endpoint, path);
                var query = parameter.ParametalizeForGet();
                if (!String.IsNullOrEmpty(query))
                {
                    endpoint += "?" + query;
                }
                client = new TwitterApiHttpClient(Credential, Proxy, UserAgent, false);
                client.Timeout = Timeout.InfiniteTimeSpan;
                using (var resp = await client.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken).ConfigureAwait(false))
                using (var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    // winding data from user stream
                    await streamReader(stream).ConfigureAwait(false);
                }
            }
            finally
            {
                if (client != null)
                {
                    client.CancelPendingRequests();
                    client.Dispose();
                }
            }

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
        private sealed class TwitterApiHttpClient : HttpClient
        {
            private const string UserAgentHeader = "User-Agent";

            public TwitterApiHttpClient([NotNull] IOAuthCredential credential,
                [CanBeNull] IWebProxy proxy, [CanBeNull] string userAgent, bool useGZip)
                : base(GetInnerHandler(credential, proxy, useGZip))
            {
                if (!String.IsNullOrEmpty(userAgent))
                {
                    DefaultRequestHeaders.Remove(UserAgentHeader);
                    DefaultRequestHeaders.Add(UserAgentHeader, userAgent);
                }
            }

            private static HttpMessageHandler GetInnerHandler([NotNull] IOAuthCredential credential,
                [CanBeNull] IWebProxy proxy, bool useGZip)
            {
                var exceptionHandler = new TwitterApiExceptionHandler(
                    new HttpClientHandler
                    {
                        AutomaticDecompression = useGZip
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

        public void Dispose()
        {
            if (_disposed) return;
            if (_client.IsValueCreated)
            {
                var client = _client.Value;
                client.CancelPendingRequests();
                client.Dispose();
            }
            _disposed = true;
        }


    }
}
