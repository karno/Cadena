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
    public static class Lists
    {
        #region lists/show

        public static async Task<IApiResult<TwitterList>> ShowListAsync(
            [NotNull] this IApiAccessor accessor,
            [NotNull] ListParameter targetList, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            return await accessor.GetAsync("lists/show.json", targetList.ToDictionary(),
                ResultHandlers.ReadAsListAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region lists/list

        public static async Task<IApiResult<IEnumerable<TwitterList>>> GetListsAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] UserParameter targetUser,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));

            return await accessor.GetAsync("lists/list.json", targetUser.ToDictionary(),
                ResultHandlers.ReadAsListCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region lists/statuses

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> GetListTimelineAsync(
            [NotNull] this IApiAccessor accessor,
            [NotNull] ListParameter listTarget, long? sinceId, long? maxId, int? count, bool? includeRts,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>()
            {
                {"since_id", sinceId},
                {"max_id", maxId},
                {"count", count},
                {"include_rts", includeRts},
            }.ApplyParameter(listTarget).SetExtended();
            return await accessor.GetAsync("lists/statuses.json", param,
                ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region lists/members

        public static async Task<IApiResult<ICursorResult<IEnumerable<TwitterUser>>>> GetListMembersAsync(
            [NotNull] this IApiAccessor accessor,
            [NotNull] ListParameter targetList, long? cursor, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (targetList == null) throw new ArgumentNullException(nameof(targetList));
            var param = new Dictionary<string, object>()
            {
                {"cursor", cursor},
                {"skip_status", true},
            }.ApplyParameter(targetList);
            return await accessor.GetAsync("lists/members.json", param,
                ResultHandlers.ReadAsCursoredUsersAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region lists/memberships

        public static async Task<IApiResult<ICursorResult<IEnumerable<TwitterList>>>> GetListMembershipsAsync(
            [NotNull] this IApiAccessor accessor,
            [NotNull] ListParameter targetList, long? cursor, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (targetList == null) throw new ArgumentNullException(nameof(targetList));
            var param = new Dictionary<string, object>()
            {
                {"cursor", cursor},
            }.ApplyParameter(targetList);
            return await accessor.GetAsync("lists/memberships.json", param,
                ResultHandlers.ReadAsCursoredListsAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
