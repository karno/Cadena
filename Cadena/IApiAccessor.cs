using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena
{
    public interface IApiAccessor : IDisposable
    {
        /// <summary>
        /// ID of the authenticated user
        /// </summary>
        long Id { get; }

        [NotNull]
        Task<IApiResult<T>> GetAsync<T>([NotNull] string path,
            [NotNull] IDictionary<string, object> parameter,
            [NotNull] Func<HttpResponseMessage, Task<T>> converter,
            CancellationToken cancellationToken);

        [NotNull]
        Task<IApiResult<T>> PostAsync<T>([NotNull] string path,
            [NotNull] HttpContent content,
            [NotNull] Func<HttpResponseMessage, Task<T>> converter,
            CancellationToken cancellationToken);

        [NotNull]
        Task<IApiResult<T>> PostAsync<T>([NotNull] string path,
            [NotNull] IDictionary<string, object> parameter,
            [NotNull] Func<HttpResponseMessage, Task<T>> converter,
            CancellationToken cancellationToken);

        [NotNull]
        Task ConnectStreamAsync([NotNull] string path,
            [NotNull] IDictionary<string, object> parameter,
            [NotNull] Func<Stream, Task> streamReader,
            CancellationToken cancellationToken);
    }
}