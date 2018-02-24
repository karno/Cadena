using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Data;
using Cadena._Internals;
using JetBrains.Annotations;

namespace Cadena.Api.Rest
{
    public static class Relations
    {
        #region friends/ids

        public static async Task<IApiResult<ICursorResult<IEnumerable<long>>>> GetFriendsIdsAsync(
            [NotNull] this IApiAccessor accessor, [CanBeNull] UserParameter nullableTargetUser,
            long? cursor, int? count, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                { "cursor", cursor },
                { "count", count }
            }.ApplyParameter(nullableTargetUser);
            return await accessor.GetAsync("friends/ids.json", param,
                ResultHandlers.ReadAsCursoredIdsAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion friends/ids

        #region followers/ids

        public static async Task<IApiResult<ICursorResult<IEnumerable<long>>>> GetFollowersIdsAsync(
            [NotNull] this IApiAccessor accessor,
            [CanBeNull] UserParameter nullableTargetUser, long? cursor, int? count, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                { "cursor", cursor },
                { "count", count }
            }.ApplyParameter(nullableTargetUser);
            return await accessor.GetAsync("followers/ids.json", param,
                ResultHandlers.ReadAsCursoredIdsAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion followers/ids

        #region friendships/no_retweets/ids

        public static async Task<IApiResult<IEnumerable<long>>> GetNoRetweetsIdsAsync(
            [NotNull] this IApiAccessor accessor,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            return await accessor.GetAsync("friendships/no_retweets/ids.json",
                new Dictionary<string, object>(), ResultHandlers.ReadAsIdCollectionAsync,
                cancellationToken).ConfigureAwait(false);
        }

        #endregion friendships/no_retweets/ids

        #region mutes/users/ids

        public static async Task<IApiResult<ICursorResult<IEnumerable<long>>>> GetMuteIdsAsync(
            [NotNull] this IApiAccessor accessor,
            long? cursor, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                { "cursor", cursor }
            };
            return await accessor.GetAsync("mutes/users/ids.json", param,
                ResultHandlers.ReadAsCursoredIdsAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion mutes/users/ids

        #region friendships/create

        public static async Task<IApiResult<TwitterUser>> CreateFriendshipAsync(
            [NotNull] this IApiAccessor accessor,
            [NotNull] UserParameter targetUser, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            return await accessor.PostAsync("friendships/create.json", targetUser.ToDictionary(),
                ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion friendships/create

        #region friendships/destroy

        public static async Task<IApiResult<TwitterUser>> DestroyFriendshipAsync(
            [NotNull] this IApiAccessor accessor,
            [NotNull] UserParameter targetUser, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            return await accessor.PostAsync("friendships/destroy.json", targetUser.ToDictionary(),
                ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion friendships/destroy

        #region friendships/show

        public static async Task<IApiResult<TwitterFriendship>> ShowFriendshipAsync(
            [NotNull] this IApiAccessor accessor,
            [NotNull] UserParameter sourceUser, [NotNull] UserParameter targetUser, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (sourceUser == null) throw new ArgumentNullException(nameof(sourceUser));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            sourceUser.SetKeyAsSource();
            targetUser.SetKeyAsTarget();
            var param = sourceUser.ToDictionary().ApplyParameter(targetUser);
            return await accessor.GetAsync("friendships/show.json", param,
                ResultHandlers.ReadAsFriendshipAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion friendships/show

        #region friendships/update

        public static async Task<IApiResult<TwitterFriendship>> UpdateFriendshipAsync(
            [NotNull] this IApiAccessor accessor,
            [NotNull] UserParameter screenName, bool? enableDeviceNotifications, bool? showRetweet,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (screenName == null) throw new ArgumentNullException(nameof(screenName));

            var param = new Dictionary<string, object>
            {
                { "device", enableDeviceNotifications },
                { "retweets", showRetweet },
            }.ApplyParameter(screenName);
            return await accessor.PostAsync("friendships/update.json", param,
                ResultHandlers.ReadAsFriendshipAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion friendships/update

        #region mutes/users/[create|destroy]

        public static async Task<IApiResult<TwitterUser>> UpdateMuteAsync(
            [NotNull] this IApiAccessor accessor,
            [NotNull] UserParameter targetUser, bool mute, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            var endpoint = mute ? "mutes/users/create" : "mutes/users/destroy";
            return await accessor.PostAsync(endpoint, targetUser.ToDictionary(),
                ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion mutes/users/[create|destroy]
    }
}