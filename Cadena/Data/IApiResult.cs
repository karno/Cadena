using System;

namespace Cadena.Data
{
    /// <summary>
    /// Twitter API result
    /// </summary>
    /// <typeparam name="T">type of result item</typeparam>
    public interface IApiResult<out T>
    {
        /// <summary>
        /// Rate limit description
        /// </summary>
        RateLimitDescription RateLimit { get; }

        /// <summary>
        /// API Result
        /// </summary>
        T Result { get; }
    }

    /// <summary>
    /// Rate limit description
    /// </summary>
    public struct RateLimitDescription
    {
        public static readonly RateLimitDescription Empty = new RateLimitDescription();

        /// <summary>
        /// Initialize rate limit description
        /// </summary>
        /// <param name="limit">limit count</param>
        /// <param name="remain">remain of rate limit</param>
        /// <param name="reset">reset time</param>
        public RateLimitDescription(long limit, long remain, DateTime reset)
        {
            Limit = limit;
            Remain = remain;
            Reset = reset;
        }

        /// <summary>
        /// Limit count
        /// </summary>
        public long Limit { get; }

        /// <summary>
        /// Remain count of rate limit
        /// </summary>
        public long Remain { get; }

        /// <summary>
        /// Reset time of rate limit
        /// </summary>
        public DateTime Reset { get; }
    }
}
