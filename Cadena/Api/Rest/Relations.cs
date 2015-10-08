using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internals;
using Cadena.Api.Parameters;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Api.Rest
{
    public static class Relations
    {
        #region friends/ids

        public static async Task<IApiResult<ICursorResult<IEnumerable<long>>>> GetFriendsIdsAsync(
            [NotNull] this IApiAccess access,
            [CanBeNull] UserParameter nullableTargetUser, long? cursor, int? count, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object>
            {
                {"cursor", cursor},
                {"count", count}
            }.ApplyParameter(nullableTargetUser);
            return await access.GetAsync("friends/ids.json", param,
                ResultHandlers.ReadAsCursoredIdsAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region followers/ids

        public static async Task<IApiResult<ICursorResult<IEnumerable<long>>>> GetFollowersIdsAsync(
            [NotNull] this IApiAccess access,
            [CanBeNull] UserParameter nullableTargetUser, long? cursor, int? count, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object>
            {
                {"cursor", cursor},
                {"count", count}
            }.ApplyParameter(nullableTargetUser);
            return await access.GetAsync("followers/ids.json", param,
                ResultHandlers.ReadAsCursoredIdsAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region friendships/no_retweets/ids

        public static async Task<IApiResult<IEnumerable<long>>> GetNoRetweetsIdsAsync(
            [NotNull] this IApiAccess access,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            return await access.GetAsync("friendships/no_retweets/ids.json",
                new Dictionary<string, object>(), ResultHandlers.ReadAsIdCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region mutes/users/ids

        public static async Task<IApiResult<ICursorResult<IEnumerable<long>>>> GetMuteIdsAsync(
            [NotNull] this IApiAccess access,
            long? cursor, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object>
            {
                {"cursor", cursor}
            };
            return await access.GetAsync("mutes/users/ids.json", param,
                ResultHandlers.ReadAsCursoredIdsAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region friendships/create

        public static async Task<IApiResult<TwitterUser>> CreateFriendshipAsync(
            [NotNull] this IApiAccess access,
            [NotNull] UserParameter targetUser, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            return await access.PostAsync("friendships/create.json", targetUser.ToDictionary(),
                ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region friendships/destroy

        public static async Task<IApiResult<TwitterUser>> DestroyFriendshipAsync(
            [NotNull] this IApiAccess access,
            [NotNull] UserParameter targetUser, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            return await access.PostAsync("friendships/destroy.json", targetUser.ToDictionary(),
                ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region friendships/show 

        public static async Task<IApiResult<TwitterFriendship>> ShowFriendshipAsync(
            [NotNull] this IApiAccess access,
            [NotNull] UserParameter sourceUser, [NotNull] UserParameter targetUser, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (sourceUser == null) throw new ArgumentNullException(nameof(sourceUser));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            sourceUser.SetKeyAsSource();
            targetUser.SetKeyAsTarget();
            var param = sourceUser.ToDictionary().ApplyParameter(targetUser);
            return await access.GetAsync("friendships/show.json", param,
                ResultHandlers.ReadAsFriendshipAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region friendships/update

        public static async Task<IApiResult<TwitterFriendship>> UpdateFriendshipAsync(
            [NotNull] this IApiAccess access,
            [NotNull] UserParameter screenName, bool? enableDeviceNotifications, bool? showRetweet,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (screenName == null) throw new ArgumentNullException(nameof(screenName));

            var param = new Dictionary<string, object>
            {
                {"device", enableDeviceNotifications},
                {"retweets", showRetweet},
            }.ApplyParameter(screenName);
            return await access.PostAsync("friendships/update.json", param,
                ResultHandlers.ReadAsFriendshipAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region mutes/users/[create|destroy]

        public static async Task<IApiResult<TwitterUser>> UpdateMuteAsync(
            [NotNull] this IApiAccess access,
            [NotNull] UserParameter targetUser, bool mute, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            var endpoint = mute ? "mutes/users/create" : "mutes/users/destroy";
            return await access.PostAsync(endpoint, targetUser.ToDictionary(),
                ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}

