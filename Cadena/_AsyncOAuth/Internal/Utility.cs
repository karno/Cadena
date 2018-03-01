// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//
// ! This file is a part of assets of AsyncOAuth project.
//
// AsyncOAuth - https://github.com/neuecc/AsyncOAuth from @neuecc
// This file is licensed under the MIT License. Check above link for more detail.
//
// Note: Several modifications may have been applied from original source by @ karno.
//
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using System.Collections.Generic;
using System.Linq;

namespace AsyncOAuth
{
    internal static class Utility
    {
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTime(this DateTime target)
        {
            return (long)(target - unixEpoch).TotalSeconds;
        }

        /// <summary>Escape RFC3986 String</summary>
        public static string UrlEncode(this string stringToEscape)
        {
            return Uri.EscapeDataString(EnsurePercentEncodingUpperCase(stringToEscape))
                      .Replace("!", "%21")
                      .Replace("*", "%2A")
                      .Replace("'", "%27")
                      .Replace("(", "%28")
                      .Replace(")", "%29");
        }

        private static string EnsurePercentEncodingUpperCase(string escaped)
        {
            var ca = escaped.ToCharArray();
            for (var i = 0; i < ca.Length; i++)
            {
                if (ca[i] != '%') continue;
                for (var j = 1; j < 3; j++)
                {
                    if ('a' <= ca[i + j] && ca[i + j] <= 'f')
                    {
                        ca[i + j] -= (char)32;
                    }
                }
            }
            return new String(ca);
        }

        public static string UrlDecode(this string stringToUnescape)
        {
            return Uri.UnescapeDataString(stringToUnescape.Replace("+", " "))
                      .Replace("%21", "!")
                      .Replace("%2A", "*")
                      .Replace("%27", "'")
                      .Replace("%28", "(")
                      .Replace("%29", ")");
        }

        public static IEnumerable<KeyValuePair<string, string>> SplitQueryString(string query)
        {
            return query.TrimStart('?').Split('&')
                        .Where(x => !String.IsNullOrEmpty(x))
                        .Select(x =>
                        {
                            var xs = x.Split('=');
                            return new KeyValuePair<string, string>(xs[0], xs[1]);
                        });
        }

        public static string Wrap(this string input, string wrapper)
        {
            return wrapper + input + wrapper;
        }

        public static string ToString<T>(this IEnumerable<T> source, string separator)
        {
            return string.Join(separator, source);
        }
    }
}