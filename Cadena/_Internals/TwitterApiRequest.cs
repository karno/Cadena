using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Data;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena._Internals
{
    /// <summary>
    /// Request to Twitter with designated parameters.
    /// </summary>
    internal static class TwitterApiRequest
    {
        public static async Task<IApiResult<T>> PostAsync<T>([NotNull] this IApiAccess access,
           [NotNull] string path, [NotNull] HttpContent content,
           [NotNull] Func<HttpResponseMessage, Task<T>> converter,
           CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            using (var client = access.CreateOAuthClient())
            using (var resp = await client.PostAsync(access.AccessConfiguration, path, content,
                cancellationToken).ConfigureAwait(false))
            {
                return await converter(resp).ToApiResult(resp).ConfigureAwait(false);
            }
        }

        public static async Task<IApiResult<T>> PostAsync<T>([NotNull] this IApiAccess access,
           [NotNull] string path, [NotNull] IDictionary<string, object> parameter,
           [NotNull] Func<HttpResponseMessage, Task<T>> converter,
           CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            using (var client = access.CreateOAuthClient())
            using (var resp = await client.PostAsync(access.AccessConfiguration, path, parameter,
                cancellationToken).ConfigureAwait(false))
            {
                return await converter(resp).ToApiResult(resp).ConfigureAwait(false);
            }
        }

        public static async Task<IApiResult<T>> GetAsync<T>([NotNull] this IApiAccess access,
           [NotNull] string path, [NotNull] IDictionary<string, object> parameter,
            [NotNull] Func<HttpResponseMessage, Task<T>> converter,
           CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            using (var client = access.CreateOAuthClient())
            using (var resp = await client.GetAsync(access.AccessConfiguration, path, parameter,
                cancellationToken).ConfigureAwait(false))
            {
                return await converter(resp).ToApiResult(resp).ConfigureAwait(false);
            }
        }

        private static async Task<IApiResult<T>> ToApiResult<T>([NotNull] this Task<T> result,
            [NotNull] HttpResponseMessage msg)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (msg == null) throw new ArgumentNullException(nameof(msg));

            return ApiResult.Create(await result.ConfigureAwait(false), msg);
        }

        private static Task<HttpResponseMessage> GetAsync([NotNull] this HttpClient client,
           [NotNull] IApiAccessConfiguration properties, [NotNull] string path,
           [NotNull] IDictionary<string, object> parameter,
           CancellationToken cancellationToken)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            var url = FormatUrl(properties.Endpoint, path, parameter.ParametalizeForGet());
            Debug.WriteLine("[GET] " + url);
            return client.GetAsync(url, cancellationToken);
        }

        private static Task<HttpResponseMessage> PostAsync([NotNull] this HttpClient client,
           [NotNull] IApiAccessConfiguration properties, [NotNull] string path,
           [NotNull] IDictionary<string, object> parameter,
           CancellationToken cancellationToken)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            return client.PostAsync(properties, path,
                parameter.ParametalizeForPost(), cancellationToken);
        }

        private static Task<HttpResponseMessage> PostAsync([NotNull] this HttpClient client,
           [NotNull] IApiAccessConfiguration properties, [NotNull] string path, [NotNull] HttpContent content,
           CancellationToken cancellationToken)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (content == null) throw new ArgumentNullException(nameof(content));
            var url = FormatUrl(properties.Endpoint, path);
            Debug.WriteLine("[POST] " + url);
            return client.PostAsync(url, content, cancellationToken);
        }

        private static string FormatUrl([NotNull] string endpoint, [NotNull] string path)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (path == null) throw new ArgumentNullException(nameof(path));
            return HttpUtility.ConcatUrl(endpoint, path);
        }

        private static string FormatUrl([NotNull] string endpoint, [NotNull] string path, [NotNull] string param)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (param == null) throw new ArgumentNullException(nameof(param));
            return String.IsNullOrEmpty(param)
                ? FormatUrl(endpoint, path)
                : FormatUrl(endpoint, path) + "?" + param;
        }
    }
}
