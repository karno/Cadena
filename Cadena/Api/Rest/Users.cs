using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internals;
using Cadena.Api.Parameters;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Api.Rest
{
    public static class Users
    {
        #region users/lookup

        public static Task<IApiResult<IEnumerable<TwitterUser>>> LookupUserAsync(
            [NotNull] this ApiAccessor accessor, [NotNull] IEnumerable<long> userIds,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (userIds == null) throw new ArgumentNullException(nameof(userIds));
            return LookupUserCoreAsync(accessor, userIds, null, cancellationToken);
        }

        public static Task<IApiResult<IEnumerable<TwitterUser>>> LookupUserAsync(
            [NotNull] this ApiAccessor accessor, [NotNull] IEnumerable<string> screenNames,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (screenNames == null) throw new ArgumentNullException(nameof(screenNames));
            return LookupUserCoreAsync(accessor, null, screenNames, cancellationToken);
        }

        private static async Task<IApiResult<IEnumerable<TwitterUser>>> LookupUserCoreAsync(
            [NotNull] ApiAccessor accessor, IEnumerable<long> userIds, IEnumerable<string> screenNames,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var userIdsString = userIds == null
                ? null
                : String.Join(",", userIds.Select(s => s.ToString(CultureInfo.InvariantCulture)));
            var param = new Dictionary<string, object>
            {
                {"user_id", userIdsString},
                {"screen_name", screenNames == null ? null : String.Join(",", screenNames)}
            };
            return await accessor.GetAsync("users/lookup.json", param,
                ResultHandlers.ReadAsUserCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region users/search

        public static async Task<IApiResult<IEnumerable<TwitterUser>>> SearchUserAsync(
            [NotNull] this ApiAccessor accessor, [NotNull] string query, int? page, int? count,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (query == null) throw new ArgumentNullException(nameof(query));
            var param = new Dictionary<string, object>
            {
                {"q", query},
                {"page", page},
                {"count", count},
            };
            return await accessor.GetAsync("users/search.json", param,
                ResultHandlers.ReadAsUserCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region users/show

        public static async Task<IApiResult<TwitterUser>> ShowUserAsync(
            [NotNull] this ApiAccessor accessor, [NotNull] UserParameter parameter,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            var param = parameter.ToDictionary();
            return await accessor.GetAsync("users/show.json", param,
                ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
