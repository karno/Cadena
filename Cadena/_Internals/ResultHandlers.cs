using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cadena.Data;
using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena._Internals
{
    /// <summary>
    /// Methods for handling results
    /// </summary>
    internal static class ResultHandlers
    {
        internal static async Task<IApiResult<T>> ToApiResult<T>([NotNull] this Task<T> result,
            [NotNull] HttpResponseMessage msg)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (msg == null) throw new ArgumentNullException(nameof(msg));

            return ApiResult.Create(await result.ConfigureAwait(false), msg);
        }

        public static async Task<string> ReadAsStringAsync([NotNull] this HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return await response.EnsureSuccessStatusCode().Content
                                 .ReadAsStringAsync().ConfigureAwait(false);
        }

        public static Task<TwitterUser> ReadAsUserAsync([NotNull] HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return ReadAsAsync(response, d => new TwitterUser(d));
        }

        public static Task<TwitterStatus> ReadAsStatusAsync([NotNull] HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return ReadAsAsync(response, d => new TwitterStatus(d));
        }

        public static Task<TwitterList> ReadAsListAsync([NotNull] HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return ReadAsAsync(response, d => new TwitterList(d));
        }

        public static Task<TwitterConfiguration> ReadAsConfigurationAsync([NotNull] HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return ReadAsAsync(response, d => new TwitterConfiguration(d));
        }

        public static Task<TwitterSavedSearch> ReadAsSavedSearchAsync([NotNull] HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return ReadAsAsync(response, d => new TwitterSavedSearch(d));
        }

        public static Task<TwitterFriendship> ReadAsFriendshipAsync([NotNull] HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return ReadAsAsync(response, d => new TwitterFriendship(d));
        }

        private static async Task<T> ReadAsAsync<T>([NotNull] HttpResponseMessage response,
            [NotNull] Func<JsonValue, T> instantiator)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (instantiator == null) throw new ArgumentNullException(nameof(instantiator));

            var json = await response.ReadAsStringAsync().ConfigureAwait(false);
            return instantiator(MeteorJson.Parse(json));
        }

        public static Task<IEnumerable<TwitterUser>> ReadAsUserCollectionAsync(
            [NotNull] HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return ReadAsCollectionAsync(response, u => new TwitterUser(u));
        }

        public static Task<IEnumerable<TwitterList>> ReadAsListCollectionAsync(
            [NotNull] HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return ReadAsCollectionAsync(response, l => new TwitterList(l));
        }

        public static Task<IEnumerable<long>> ReadAsIdCollectionAsync([NotNull] HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return ReadAsCollectionAsync(response, d => d.AsLong());
        }

        public static Task<IEnumerable<TwitterSavedSearch>> ReadAsSavedSearchCollectionAsync(
            [NotNull] HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return ReadAsCollectionAsync(response, d => new TwitterSavedSearch(d));
        }

        public static async Task<IEnumerable<TwitterStatus>> ReadAsStatusCollectionAsync(
            [NotNull] HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            var json = await response.ReadAsStringAsync().ConfigureAwait(false);
            var parsed = MeteorJson.Parse(json);
            if (parsed.ContainsKey("statuses"))
            {
                parsed = parsed["statuses"];
            }
            return parsed.AsArray().Select(status => new TwitterStatus(status));
        }

        private static async Task<IEnumerable<T>> ReadAsCollectionAsync<T>(
            [NotNull] HttpResponseMessage response, [NotNull] Func<JsonValue, T> factory)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var json = await response.ReadAsStringAsync().ConfigureAwait(false);
            var parsed = MeteorJson.Parse(json);
            return parsed.AsArray().Select(factory);
        }

        public static Task<ICursorResult<IEnumerable<long>>> ReadAsCursoredIdsAsync(
            [NotNull] HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return ReadAsCursoredAsync(response, json => json["ids"].AsArray(), d => d.AsLong());
        }

        public static Task<ICursorResult<IEnumerable<TwitterUser>>> ReadAsCursoredUsersAsync(
            [NotNull] HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return ReadAsCursoredAsync(response, json => json["users"].AsArray(), d => new TwitterUser(d));
        }

        public static Task<ICursorResult<IEnumerable<TwitterList>>> ReadAsCursoredListsAsync(
            [NotNull] HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return ReadAsCursoredAsync(response, json => json["lists"].AsArray(), d => new TwitterList(d));
        }

        private static async Task<ICursorResult<IEnumerable<T>>> ReadAsCursoredAsync<T>(
            [NotNull] HttpResponseMessage response, [NotNull] Func<JsonValue, JsonArray> selector,
            [NotNull] Func<JsonValue, T> instantiator)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (instantiator == null) throw new ArgumentNullException(nameof(instantiator));

            var json = await response.ReadAsStringAsync().ConfigureAwait(false);
            var parsed = MeteorJson.Parse(json);
            var converteds = selector(parsed).Select(instantiator);
            var prevCursor = parsed["previous_cursor_str"].AsString() ?? "-1";
            var nextCursor = parsed["next_cursor_str"].AsString() ?? "-1";
            return CursorResult.Create(converteds, prevCursor, nextCursor);
        }
    }
}
