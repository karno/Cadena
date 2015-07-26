using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internal;
using Cadena.Api.Parameters;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Api.Rest
{
    public static class Users
    {
        #region users/lookup

        public static Task<IApiResult<IEnumerable<TwitterUser>>> LookupUserAsync(
            [NotNull] this IApiAccess access, [NotNull] IEnumerable<long> userIds,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (userIds == null) throw new ArgumentNullException(nameof(userIds));
            return LookupUserCoreAsync(access, userIds, null, cancellationToken);
        }

        public static Task<IApiResult<IEnumerable<TwitterUser>>> LookupUserAsync(
            [NotNull] this IApiAccess access, [NotNull] IEnumerable<string> screenNames,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (screenNames == null) throw new ArgumentNullException(nameof(screenNames));
            return LookupUserCoreAsync(access, null, screenNames, cancellationToken);
        }

        private static async Task<IApiResult<IEnumerable<TwitterUser>>> LookupUserCoreAsync(
            [NotNull] IApiAccess access, IEnumerable<long> userIds, IEnumerable<string> screenNames,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var userIdsString = userIds == null
                ? null
                : String.Join(",", userIds.Select(s => s.ToString(CultureInfo.InvariantCulture)));
            var param = new Dictionary<string, object>
            {
                {"user_id", userIdsString},
                {"screen_name", screenNames == null ? null : String.Join(",", screenNames)}
            };
            return await access.GetAsync("users/lookup.json", param,
                ResultHandlers.ReadAsUserCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region users/search

        public static async Task<IApiResult<IEnumerable<TwitterUser>>> SearchUserAsync(
            [NotNull] this IApiAccess access, [NotNull] string query, int? page, int? count,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (query == null) throw new ArgumentNullException(nameof(query));
            var param = new Dictionary<string, object>
            {
                {"q", query},
                {"page", page},
                {"count", count},
            };
            return await access.GetAsync("users/search.json", param,
                ResultHandlers.ReadAsUserCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region users/show

        public static async Task<IApiResult<TwitterUser>> ShowUserAsync(
            [NotNull] this IApiAccess access, [NotNull] UserParameter parameter,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            var param = parameter.ToDictionary();
            return await access.GetAsync("users/show.json", param,
                ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
