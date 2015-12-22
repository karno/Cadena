using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internals;
using Cadena.Data;

namespace Cadena.Util
{
    public static class CursorResultExtension
    {
        public delegate Task<Tuple<IApiResult<T>, ApiContinuationReader<T>>> ApiContinuationReader<T>();

        public static Task<Tuple<IApiResult<IEnumerable<T>>, ApiContinuationReader<IEnumerable<T>>>> ReadCursorApi<T>(
            this ApiAccessor accessor,
            Func<ApiAccessor, long, Task<IApiResult<ICursorResult<IEnumerable<T>>>>> reader)
        {
            return ReadCursorApi(accessor, -1, reader, CancellationToken.None);
        }

        public static Task<Tuple<IApiResult<IEnumerable<T>>, ApiContinuationReader<IEnumerable<T>>>> ReadCursorApi<T>(
            this ApiAccessor accessor,
            Func<ApiAccessor, long, Task<IApiResult<ICursorResult<IEnumerable<T>>>>> reader,
            CancellationToken cancellationToken)
        {
            return ReadCursorApi(accessor, -1, reader, cancellationToken);
        }

        private static async Task<Tuple<IApiResult<IEnumerable<T>>, ApiContinuationReader<IEnumerable<T>>>> ReadCursorApi<T>(
            this ApiAccessor accessor, long cursor,
            Func<ApiAccessor, long, Task<IApiResult<ICursorResult<IEnumerable<T>>>>> reader,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var r = await reader(accessor, cursor).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            var cr = r.Result;
            var ir = ApiResult.Create(cr.Result, r.RateLimit);
            ApiContinuationReader<IEnumerable<T>> callback = null;

            if (cr.CanReadNext)
            {
                callback = () => ReadCursorApi(accessor, cr.NextCursor, reader, cancellationToken);
            }
            return Tuple.Create(ir, callback);
        }
    }
}
