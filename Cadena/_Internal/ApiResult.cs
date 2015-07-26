using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Cadena.Data;
using Cadena.Util;

namespace Cadena._Internal
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
        public static IApiResult<T> Create<T>(T item, HttpResponseMessage message)
        {
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
        public static IApiResult<T> Create<T>(T item, RateLimitDescription description)
        {
            return new ApiResultImpl<T>(item, description.Limit, description.Remain, description.Reset);
        }

        private static string GetFirstHeaderOrNull(HttpResponseMessage message, string key)
        {
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
                this.RateLimit = new RateLimitDescription(limit, remain, reset);
                this.Result = result;
            }

            public RateLimitDescription RateLimit { get; }

            public T Result { get; }
        }
    }
}