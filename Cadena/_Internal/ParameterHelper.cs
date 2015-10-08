﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Cadena.Api;
using JetBrains.Annotations;

namespace Cadena._Internal
{
    /// <summary>
    /// Helper methods for ParameterBase.
    /// </summary>
    internal static class ParameterHelper
    {
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
                    .Select(kvp => $"{kvp.Key}={EncodeForParameters(kvp.Value.ToString())}"));
        }

        internal static IDictionary<string, object> ApplyParameter(
            [NotNull] this IDictionary<string, object> dict,
            [CanBeNull] ParameterBase paramOrNull)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));

            paramOrNull?.SetDictionary(dict);
            return dict;
        }

        private static string EncodeForParameters([NotNull] string value)
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
    }
}