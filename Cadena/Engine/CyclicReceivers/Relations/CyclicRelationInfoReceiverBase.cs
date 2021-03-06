﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Data;
using Cadena.Util;
using Cadena._Internals;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Relations
{
    public abstract class CyclicRelationInfoReceiverBase : CyclicReceiverBase
    {
        private readonly Action<IEnumerable<long>> _handler;
        protected override long MinimumIntervalTicks => TimeSpan.FromHours(6).Ticks;

        protected CyclicRelationInfoReceiverBase([NotNull] Action<IEnumerable<long>> handler,
            [CanBeNull] Action<Exception> exceptionHandler) : base(exceptionHandler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        protected static async Task<IApiResult<IEnumerable<T>>> RetrieveCursoredResult<T>(IApiAccessor accessor,
            Func<IApiAccessor, long, Task<IApiResult<ICursorResult<IEnumerable<T>>>>> func,
            Action<Exception> exceptionHandler, CancellationToken token)
        {
            var resultList = new List<T>();
            CursorResultExtension.ApiContinuationReader<IEnumerable<T>> reader =
                () => accessor.ReadCursorApi(func, token);
            var lastRateLimit = RateLimitDescription.Empty;
            while (reader != null)
            {
                try
                {
                    var result = await reader().ConfigureAwait(false);
                    resultList.AddRange(result.Item1.Result);
                    lastRateLimit = result.Item1.RateLimit;
                    reader = result.Item2;
                }
                catch (Exception ex)
                {
                    exceptionHandler(ex);
                    break;
                }
            }
            return ApiResult.Create(resultList, lastRateLimit);
        }

        protected void CallHandler([CanBeNull] IEnumerable<long> result)
        {
            if (result == null) return;
            if (result is long[] array && array.Length == 0) return;
            if (result is ICollection collection && collection.Count == 0) return;
            _handler(result);
        }
    }
}