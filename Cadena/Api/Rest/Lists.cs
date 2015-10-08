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
            [NotNull] this IApiAccess access,
            [NotNull] ListParameter targetList, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            return await access.GetAsync("lists/show.json", targetList.ToDictionary(),
                ResultHandlers.ReadAsListAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region lists/list

        public static async Task<IApiResult<IEnumerable<TwitterList>>> GetListsAsync(
            [NotNull] this IApiAccess access,
            [NotNull] ListParameter targetList, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (targetList == null) throw new ArgumentNullException(nameof(targetList));

            return await access.GetAsync("lists/list.json", targetList.ToDictionary(),
                ResultHandlers.ReadAsListCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region lists/statuses

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> GetListTimelineAsync(
            [NotNull] this IApiAccess access,
            [NotNull] ListParameter listTarget, long? sinceId, long? maxId, int? count, bool? includeRts,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object>()
            {
                {"since_id", sinceId},
                {"max_id", maxId},
                {"count", count},
                {"include_rts", includeRts},
            }.ApplyParameter(listTarget);
            return await access.GetAsync("lists/statuses.json", param,
                ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Memberships

        public static async Task<IApiResult<ICursorResult<IEnumerable<TwitterUser>>>> GetListMembersAsync(
            [NotNull] this IApiAccess access,
            [NotNull] ListParameter targetList, long? cursor, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (targetList == null) throw new ArgumentNullException(nameof(targetList));
            var param = new Dictionary<string, object>()
            {
                {"cursor", cursor},
                {"skip_status", true},
            }.ApplyParameter(targetList);
            return await access.GetAsync("lists/members.json", param,
                ResultHandlers.ReadAsCursoredUsersAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
