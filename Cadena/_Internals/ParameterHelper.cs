using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Cadena.Api;
using JetBrains.Annotations;

namespace Cadena._Internals
{
    /// <summary>
    /// Helper methods for ParameterBase.
    /// </summary>
    internal static class ParameterHelper
    {
        internal static IDictionary<string, object> CreateEmpty()
        {
            return new Dictionary<string, object>();
        }

        private const string ParameterAllowedChars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        internal static FormUrlEncodedContent ParametalizeForPost(
            [NotNull] this IDictionary<string, object> dict)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));

            return new FormUrlEncodedContent(
                dict.Where(kvp => kvp.Value != null)
                    .Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value.ToString())));
        }

        internal static string ParametalizeForGet([NotNull] this IDictionary<string, object> dict)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));

            return String.Join("&",
                dict.Where(kvp => kvp.Value != null)
                    .OrderBy(kvp => kvp.Key)
                    .Select(kvp => $"{kvp.Key}={EncodeParameters(kvp.Value.ToString())}"));
        }

        internal static IDictionary<string, object> ApplyParameter(
            [NotNull] this IDictionary<string, object> dict,
            [CanBeNull] ParameterBase paramOrNull)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));

            paramOrNull?.SetDictionary(dict);
            return dict;
        }

        internal static IDictionary<string, object> SetExtended(
            [NotNull] this IDictionary<string, object> dict)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));

            dict.Add("tweet_mode", "extended");
            return dict;
        }

        private static string EncodeParameters([NotNull] string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var result = new StringBuilder();
            var data = Encoding.UTF8.GetBytes(value);
            var len = data.Length;

            for (var i = 0; i < len; i++)
            {
                int c = data[i];
                if (c < 0x80 && ParameterAllowedChars.IndexOf((char)c) != -1)
                {
                    result.Append((char)c);
                }
                else
                {
                    result.Append('%' + $"{data[i]:x2}");
                }
            }
            return result.ToString();
        }

        internal static string JoinString(
            [NotNull] this IEnumerable<string> strings, [NotNull] string separator)
        {
            if (strings == null) throw new ArgumentNullException(nameof(strings));
            if (separator == null) throw new ArgumentNullException(nameof(separator));
            return String.Join(separator, strings);
        }
    }
}
