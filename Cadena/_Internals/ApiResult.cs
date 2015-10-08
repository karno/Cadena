using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Cadena.Data;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena._Internals
{
    /// <summary>
    /// Create IApiResult object.
    /// </summary>
    internal static class ApiResult
    {
        public static readonly string HeaderRateLimitLimit = "X-Rate-Limit-Limit";

        public static readonly string HeaderRateLimitRemaining = "X-Rate-Limit-Remaining";

        public static readonly string HeaderRateLimitReset = "X-Rate-Limit-Reset";

        /// <summary>
        /// Create IApiResult object.
        /// </summary>
        /// <typeparam name="T">type of result</typeparam>
        /// <param name="item">result item</param>
        /// <param name="message">HTTP response</param>
        /// <returns></returns>
        public static IApiResult<T> Create<T>([NotNull] T item, [NotNull] HttpResponseMessage message)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var limit = GetFirstHeaderOrNull(message, HeaderRateLimitLimit).ParseLong();
            var remain = GetFirstHeaderOrNull(message, HeaderRateLimitRemaining).ParseLong();
            var reset = GetFirstHeaderOrNull(message, HeaderRateLimitReset).ParseLong();
            return new ApiResultImpl<T>(item, limit, remain, reset);
        }

        /// <summary>
        /// Create IApiResult object with specified RateLimitDescription.
        /// </summary>
        /// <typeparam name="T">type of result</typeparam>
        /// <param name="item">result item</param>
        /// <param name="description">rate limit description</param>
        /// <returns></returns>
        public static IApiResult<T> Create<T>([NotNull] T item, RateLimitDescription description)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return new ApiResultImpl<T>(item, description.Limit, description.Remain, description.Reset);
        }

        private static string GetFirstHeaderOrNull([NotNull] HttpResponseMessage message, [NotNull] string key)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (key == null) throw new ArgumentNullException(nameof(key));

            IEnumerable<string> values;
            return message.Headers.TryGetValues(key, out values) ? values.FirstOrDefault() : null;
        }

        /// <summary>
        /// IApiResult implementation class
        /// </summary>
        /// <typeparam name="T">type of result</typeparam>
        private sealed class ApiResultImpl<T> : IApiResult<T>
        {
            public ApiResultImpl(T result, long limit, long remain, long reset)
                : this(result, limit, remain, UnixEpoch.GetDateTimeByUnixEpoch(reset))
            {
            }

            public ApiResultImpl(T result, long limit, long remain, DateTime reset)
            {
                RateLimit = new RateLimitDescription(limit, remain, reset);
                Result = result;
            }

            public RateLimitDescription RateLimit { get; }

            public T Result { get; }
        }
    }
}