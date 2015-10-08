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
        public delegate Task<Tuple<T, RestReader<T>>> RestReader<T>();

        public static Task<Tuple<IApiResult<IEnumerable<T>>, RestReader<IApiResult<IEnumerable<T>>>>> ReadCursorApi<T>(
            this IApiAccess access,
            Func<IApiAccess, long, Task<IApiResult<ICursorResult<IEnumerable<T>>>>> reader)
        {
            return ReadCursorApi(access, -1, reader, CancellationToken.None);
        }


        public static Task<Tuple<IApiResult<IEnumerable<T>>, RestReader<IApiResult<IEnumerable<T>>>>> ReadCursorApi<T>(
            this IApiAccess access,
            Func<IApiAccess, long, Task<IApiResult<ICursorResult<IEnumerable<T>>>>> reader,
            CancellationToken cancellationToken)
        {
            return ReadCursorApi(access, -1, reader, cancellationToken);
        }

        private static async Task<Tuple<IApiResult<IEnumerable<T>>, RestReader<IApiResult<IEnumerable<T>>>>> ReadCursorApi<T>(
            this IApiAccess access, long cursor,
            Func<IApiAccess, long, Task<IApiResult<ICursorResult<IEnumerable<T>>>>> reader,
            CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var r = await reader(access, cursor).ConfigureAwait(false);
                var cr = r.Result;
                var ir = ApiResult.Create(cr.Result, r.RateLimit);
                RestReader<IApiResult<IEnumerable<T>>> callback = null;

                if (cr.CanReadNext)
                {
                    callback = () => ReadCursorApi(access, cr.NextCursor, reader, cancellationToken);
                }
                return Tuple.Create(ir, callback);
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}
