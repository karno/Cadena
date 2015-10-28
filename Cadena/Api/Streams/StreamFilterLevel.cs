using System;

namespace Cadena.Api.Streams
{
    /// <summary>
    /// Describes filter level of the streams.
    /// </summary>
    public enum StreamFilterLevel
    {
        /// <summary>
        /// Filter is not applied. All tweets will be displayed.
        /// </summary>
        None,
        /// <summary>
        /// Filter is applied. Preventing you from unpleasant tweets.
        /// </summary>
        Low,
        /// <summary>
        /// Higher filter is applied. This option is suitable for displaying
        /// tweets for public(e.g. signage, live feeds, conferences, etc.)
        /// </summary>
        Middle
    }

    internal static class StreamFilterLevelExtension
    {
        public static string ToParamString(this StreamFilterLevel level)
        {
            switch (level)
            {
                case StreamFilterLevel.None:
                    return "none";
                case StreamFilterLevel.Low:
                    return "low";
                case StreamFilterLevel.Middle:
                    return "middle";
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
    }
}
