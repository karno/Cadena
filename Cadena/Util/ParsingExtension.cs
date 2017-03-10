using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Cadena.Util
{
    /// <summary>
    /// XML Node Parser
    /// </summary>
    public static class ParsingExtension
    {
        public const string TwitterDateTimeFormat = "ddd MMM d HH':'mm':'ss zzz yyyy";

        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AssertNotNull([CanBeNull] this string s, string message)
        {
            if (s == null)
            {
                throw new ArgumentException(message);
            }
            return s;
        }

        /// <summary>
        /// Parse text as bool
        /// </summary>
        /// <param name="s">convert value</param>
        /// <param name="default">default value if string is null or unacceptable value</param>
        /// <returns>converted value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ParseBool([CanBeNull] this string s, bool @default = false)
        {
            if (s == null)
            {
                return @default;
            }
            return @default ? s.ToLower() != "false" : s.ToLower() == "true";
        }

        /// <summary>
        /// Parse string as long
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ParseLong([CanBeNull] this string s)
        {
            return long.TryParse(s, out var v) ? v : 0;
        }

        /// <summary>
        /// Parse nullable id
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long? ParseNullableId([CanBeNull] this string s)
        {
            return s != null && Int64.TryParse(s, out var v) && v != 0 ? (long?)v : null;
        }

        /// <summary>
        /// Parse date time
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ParseDateTime([CanBeNull] this string s)
        {
            return s.ParseDateTime(DateTime.MinValue);
        }

        /// <summary>
        /// Parse date time
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ParseDateTime([CanBeNull] this string s, DateTime @default)
        {
            return s != null && DateTime.TryParse(s, out var dt) ? dt : @default;
        }

        /// <summary>
        /// Parse date time
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ParseDateTime([CanBeNull] this string s, [NotNull] string format)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));

            return s.ParseDateTime(format, DateTime.MinValue);
        }

        /// <summary>
        /// Parse date time
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ParseDateTime([CanBeNull] this string s,
            [NotNull] string format, DateTime @default)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));

            if (s != null &&
                DateTime.TryParseExact(s, format,
                    System.Globalization.DateTimeFormatInfo.InvariantInfo,
                    System.Globalization.DateTimeStyles.None, out var dt))
            {
                return dt;
            }
            return @default;
        }

        /// <summary>
        /// Parse date time by twitter default format
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ParseTwitterDateTime([CanBeNull] this string s)
        {
            return s.ParseDateTime(TwitterDateTimeFormat);
        }

        /// <summary>
        /// Parse string as unix serial time
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ParseUnixTime([CanBeNull] this string s)
        {
            if (s == null) return DateTime.MinValue;
            return UnixEpoch.GetDateTimeByUnixEpoch(s.ParseLong());
        }

        /// <summary>
        /// Parse uri
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Uri ParseUri([CanBeNull] this string s)
        {
            return s != null && Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out var ret) ? ret : null;
        }

        /// <summary>
        /// Parse uri as absolute uri
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Uri ParseUriAbsolute([CanBeNull] this string s)
        {
            var ret = s.ParseUri();
            if (ret == null || !ret.IsAbsoluteUri)
            {
                return null;
            }
            return ret;
        }

        /// <summary>
        /// Resolve entity-escaped string
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ResolveEntity([CanBeNull] string text)
        {
            return text
                // .Replace("&quot;", "\"")
                ?.Replace("&lt;", "<")
                 .Replace("&gt;", ">")
                 .Replace("&amp;", "&");
        }

        /// <summary>
        /// Escape string with entities
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string EscapeEntity([CanBeNull] string text)
        {
            return text
                ?.Replace("&", "&amp;")
                 .Replace(">", "&gt;")
                 .Replace("<", "&lt;");
            // .Replace("\"", "&quot;")
        }
    }
}

