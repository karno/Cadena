﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Data;
using Cadena.Meteor;
using Cadena.Util;
using Cadena._Internals;
using JetBrains.Annotations;

namespace Cadena.Api.Rest
{
    public static class Tweets
    {
        #region statuses/show

        public static async Task<IApiResult<long?>> GetMyRetweetIdOfStatusAsync(
            [NotNull] this IApiAccessor accessor, long id,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                { "id", id },
                { "include_my_retweet", true }
            };
            return await accessor.GetAsync("statuses/show.json", param,
                async resp =>
                {
                    var json = await resp.ReadAsStringAsync().ConfigureAwait(false);
                    var graph = MeteorJson.Parse(json);
                    return graph.ContainsKey("current_user_retweet")
                        ? graph["current_user_retweet"]["id_str"].AsString().ParseLong()
                        : (long?)null;
                }, cancellationToken).ConfigureAwait(false);
        }

        #endregion statuses/show

        #region statuses/retweets/:id

        public static async Task<IApiResult<IEnumerable<TwitterUser>>> GetRetweetsAsync(
            [NotNull] this IApiAccessor accessor, long id, int? count,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object> { { "count", count } };
            return await accessor.GetAsync("statuses/retweets/" + id + ".json", param,
                                     ResultHandlers.ReadAsUserCollectionAsync, cancellationToken)
                                 .ConfigureAwait(false);
        }

        #endregion statuses/retweets/:id

        #region retweeter/ids

        public static async Task<IApiResult<ICursorResult<IEnumerable<long>>>> GetRetweeterIdsAsync(
            this IApiAccessor accessor, long id, long cursor,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                { "id", id },
                { "cursor", cursor }
            };
            return await accessor.GetAsync("retweeters/ids.json", param,
                ResultHandlers.ReadAsCursoredIdsAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion retweeter/ids

        #region statuses/show

        public static async Task<IApiResult<TwitterStatus>> ShowTweetAsync(
            [NotNull] this IApiAccessor accessor, long id,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                { "id", id }
            }.SetExtended();
            return await accessor.GetAsync("statuses/show.json", param,
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion statuses/show

        #region statuses/update

        public static async Task<IApiResult<TwitterStatus>> UpdateAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] StatusParameter status,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (status == null) throw new ArgumentNullException(nameof(status));
            return await accessor.PostAsync("statuses/update.json", status.ToDictionary().SetExtended(),
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion statuses/update

        #region media/upload

        private static string MediaUploadPath = "media/upload.json";

        public static Task<IApiResult<TwitterUploadedMedia>> UploadMediaAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] byte[] image,
            CancellationToken cancellationToken)
        {
            return accessor.UploadMediaAsync(image, null, cancellationToken);
        }

        public static Task<IApiResult<TwitterUploadedMedia>> UploadMediaAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] byte[] image, [CanBeNull] long[] additionalOwners,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (image == null) throw new ArgumentNullException(nameof(image));
            // maximum image size is 5MB.
            if (image.Length > 5 * 1024 * 1024)
            {
                throw new ArgumentOutOfRangeException(nameof(image), "image file must be smaller than 5MB.");
            }
            var content = new MultipartFormDataContent
            {
                { new ByteArrayContent(image), "media", System.IO.Path.GetRandomFileName() + ".png" }
            };
            if (additionalOwners != null)
            {
                content.Add(new StringContent(additionalOwners.Select(id => id.ToString()).JoinString(",")),
                    "additional_owners");
            }

            return accessor.UploadMediaCoreAsync(content, cancellationToken);
        }

        public static Task<IApiResult<TwitterUploadedMedia>> UploadLargeMediaAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] byte[] media, [NotNull] string mimeType,
            CancellationToken cancellationToken)
        {
            return accessor.UploadLargeMediaAsync(media, mimeType, null, null, null, cancellationToken);
        }

        public static Task<IApiResult<TwitterUploadedMedia>> UploadLargeMediaAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] byte[] media, [NotNull] string mimeType,
            int? chunkSize, [CanBeNull] IProgress<int> sentBytesNotification, CancellationToken cancellationToken)
        {
            return accessor.UploadLargeMediaAsync(media, mimeType, null, chunkSize, sentBytesNotification,
                cancellationToken);
        }

        public static Task<IApiResult<TwitterUploadedMedia>> UploadLargeMediaAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] byte[] media, [NotNull] string mimeType,
            [CanBeNull] long[] additionalOwners, CancellationToken cancellationToken)
        {
            return accessor.UploadLargeMediaAsync(media, mimeType, additionalOwners, null, null, cancellationToken);
        }

        public static async Task<IApiResult<TwitterUploadedMedia>> UploadLargeMediaAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] byte[] media, [NotNull] string mimeType,
            [CanBeNull] long[] additionalOwners, int? chunkSize, [CanBeNull] IProgress<int> sentBytesCallback,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (media == null) throw new ArgumentNullException(nameof(media));
            if (mimeType == null) throw new ArgumentNullException(nameof(mimeType));

            var mediaType = MediaFileUtility.GetMediaType(media);

            // maximum video size is 15MB (maximum image is 5MB)
            if (media.Length > 15 * 1024 * 1024)
            {
                throw new ArgumentOutOfRangeException(nameof(media), "media file must be smaller than 5MB.");
            }

            // check the bit array could be chunked
            var csize = chunkSize ?? 5 * 1024 * 1024;
            if (!mediaType.IsMovie() && media.Length <= csize)
            {
                // this item is not needed to split it into pieces
                return await accessor.UploadMediaAsync(media, additionalOwners, cancellationToken)
                                     .ConfigureAwait(false);
            }
            if (csize < 1 || media.Length / csize > 999)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkSize), "chunk size is not appropriate.");
            }

            // chunking media
            var chunks = media.Select((b, i) => new { Data = b, Index = i / csize })
                              .GroupBy(b => b.Index)
                              .Select(g => g.Select(b => b.Data).ToArray())
                              .ToArray();

            // send INIT request
            var initialContent = new MultipartFormDataContent
            {
                { new StringContent("INIT"), "command" },
                { new StringContent(media.Length.ToString()), "total_bytes" },
                { new StringContent(mimeType), "media_type" }
            };
            if (additionalOwners != null)
            {
                initialContent.Add(new StringContent(additionalOwners.Select(id => id.ToString()).JoinString(",")),
                    "additional_owners");
            }
            var initialResult =
                await accessor.UploadMediaCoreAsync(initialContent, cancellationToken).ConfigureAwait(false);

            // read initial result and prepare sending content
            var mediaId = initialResult.Result.MediaId;
            var fileName = System.IO.Path.GetRandomFileName();

            var index = 0;
            var sentSize = 0;

            // send APPEND request for uploading contents
            foreach (var part in chunks)
            {
                var content = new MultipartFormDataContent
                {
                    { new StringContent("APPEND"), "command" },
                    { new StringContent(mediaId.ToString()), "media_id" },
                    { new ByteArrayContent(part), "media", fileName },
                    { new StringContent(index.ToString()), "segment_index" }
                };
                await UploadCoreAsync(accessor, content, cancellationToken).ConfigureAwait(false);
                sentSize += part.Length;
                sentBytesCallback?.Report(sentSize);
                index++;
            }

            // send FINALIZE
            var finalContent = new MultipartFormDataContent
            {
                { new StringContent("FINALIZE"), "command" },
                { new StringContent(mediaId.ToString()), "media_id" }
            };
            return await UploadMediaCoreAsync(accessor, finalContent, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<IApiResult<TwitterUploadedMedia>> UploadMediaCoreAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] HttpContent content,
            CancellationToken cancellationToken)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            return await accessor.PostAsync(MediaUploadPath, content,
                async resp =>
                {
                    var json = await resp.ReadAsStringAsync().ConfigureAwait(false);
                    return new TwitterUploadedMedia(MeteorJson.Parse(json));
                }, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<IApiResult<string>> UploadCoreAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] HttpContent content,
            CancellationToken cancellationToken)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            return await accessor.PostAsync(MediaUploadPath, content,
                async resp => await resp.ReadAsStringAsync().ConfigureAwait(false),
                cancellationToken).ConfigureAwait(false);
        }

        #endregion media/upload

        #region statuses/destroy/:id

        public static async Task<IApiResult<TwitterStatus>> DestroyAsync(
            [NotNull] this IApiAccessor accessor, long id,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            return await accessor.PostAsync("statuses/destroy/" + id + ".json",
                ParameterHelper.CreateEmpty().SetExtended(),
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion statuses/destroy/:id

        #region statuses/retweet/:id

        public static async Task<IApiResult<TwitterStatus>> RetweetAsync(
            [NotNull] this IApiAccessor accessor, long id,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            return await accessor.PostAsync("statuses/retweet/" + id + ".json",
                ParameterHelper.CreateEmpty().SetExtended(),
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion statuses/retweet/:id
    }
}