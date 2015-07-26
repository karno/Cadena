using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internal;
using Cadena.Api.Parameters;
using Cadena.Data;
using Codeplex.Data;
using JetBrains.Annotations;

namespace Cadena.Api.Rest
{
    public static class Tweets
    {
        #region statuses/show


        public static async Task<IApiResult<long?>> GetMyRetweetIdOfStatusAsync(
            [NotNull] this IApiAccess access, long id,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object>
            {
                {"id", id},
                {"include_my_retweet", true}
            };
            return await access.GetAsync("statuses/show.json", param,
                (Func<HttpResponseMessage, Task<long?>>)(async resp =>
                {
                    var json = await resp.ReadAsStringAsync().ConfigureAwait(false);
                    var graph = DynamicJson.Parse(json);
                    return ((bool)graph.current_user_retweet()) ? Int64.Parse(graph.current_user_retweet.id_str) : null;
                }), cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region statuses/retweets/:id

        public static async Task<IApiResult<IEnumerable<TwitterUser>>> GetRetweetsAsync(
            [NotNull] this IApiAccess access, long id, int? count,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object> { { "count", count } };
            return await access.GetAsync("statuses/retweets/" + id + ".json", param,
                ResultHandlers.ReadAsUserCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region retweeter/ids

        public static async Task<IApiResult<ICursorResult<IEnumerable<long>>>> GetRetweeterIdsAsync(
            this IApiAccess access, long id, long cursor,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object>
            {
                {"id", id},
                {"cursor", cursor}
            };
            return await access.GetAsync("retweeters/ids.json", param,
                ResultHandlers.ReadAsCursoredIdsAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region statuses/show

        public static async Task<IApiResult<TwitterStatus>> ShowTweetAsync(
            [NotNull] this IApiAccess access, long id,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object>
            {
                {"id", id},
            };
            return await access.GetAsync("statuses/show.json", param,
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region statuses/update

        public static async Task<IApiResult<TwitterStatus>> UpdateAsync(
            [NotNull] this IApiAccess access, [NotNull] StatusParameter status,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (status == null) throw new ArgumentNullException(nameof(status));
            return await access.PostAsync("statuses/update.json", status.ToDictionary(),
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region media/upload

        public static async Task<IApiResult<dynamic>> UploadMediaAsync(
            [NotNull] this IApiAccess access, [NotNull] byte[] image,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (image == null) throw new ArgumentNullException(nameof(image));
            var content = new MultipartFormDataContent
            {
                {new ByteArrayContent(image), "media", System.IO.Path.GetRandomFileName() + ".png"}
            };

            return await access.PostAsync("media/upload.json", content, async resp =>
            {
                var json = await resp.ReadAsStringAsync().ConfigureAwait(false);
                return long.Parse(DynamicJson.Parse(json).media_id_string);
            }, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region statuses/destroy/:id

        public static async Task<IApiResult<TwitterStatus>> DestroyAsync(
            [NotNull] this IApiAccess access, long id,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            return await access.PostAsync("statuses/destroy/" + id + ".json",
                new Dictionary<string, object>(), ResultHandlers.ReadAsStatusAsync, cancellationToken)
                                   .ConfigureAwait(false);
        }

        #endregion

        #region statuses/retweet/:id

        public static async Task<IApiResult<TwitterStatus>> RetweetAsync(
            [NotNull] this IApiAccess access, long id,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            return await access.PostAsync("statuses/retweet/" + id + ".json",
                new Dictionary<string, object>(), ResultHandlers.ReadAsStatusAsync, cancellationToken)
                                   .ConfigureAwait(false);
        }

        #endregion
    }
}

