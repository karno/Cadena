using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internals;
using Cadena.Data;
using Cadena.Twitter.Parameters;
using JetBrains.Annotations;

namespace Cadena.Twitter.Rest
{
    public static class Favorites
    {
        #region favorites/list

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> GetFavoritesAsync(
            [NotNull] this ApiAccessor accessor, [CanBeNull] UserParameter targetUser,
            int? count, long? sinceId, long? maxId,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"count", count},
                {"since_id", sinceId},
                {"max_id", maxId},
            }.ApplyParameter(targetUser);
            return await accessor.GetAsync("favorites/list.json", param,
                ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region favorites/create

        public static async Task<IApiResult<TwitterStatus>> CreateFavoriteAsync(
            [NotNull] this ApiAccessor accessor, long id,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"id", id}
            };
            return await accessor.PostAsync("favorites/create.json", param,
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region favorites/destroy

        public static async Task<IApiResult<TwitterStatus>> DestroyFavoriteAsync(
            [NotNull] this ApiAccessor accessor, long id,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"id", id}
            };
            return await accessor.PostAsync("favorites/destroy.json", param,
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
