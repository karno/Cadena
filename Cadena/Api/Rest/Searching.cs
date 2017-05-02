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
    public static class Searching
    {
        #region search/tweets

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> SearchAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] SearchParameter query,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (query == null) throw new ArgumentNullException(nameof(query));

            return await accessor.GetAsync("search/tweets.json", query.ToDictionary().SetExtended(),
                                     ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken)
                                 .ConfigureAwait(false);
        }

        #endregion search/tweets

        #region saved_searches/list

        public static async Task<IApiResult<IEnumerable<TwitterSavedSearch>>> GetSavedSearchesAsync(
            [NotNull] this IApiAccessor accessor, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            return await accessor.GetAsync("saved_searches/list.json", new Dictionary<string, object>(),
                                     ResultHandlers.ReadAsSavedSearchCollectionAsync, cancellationToken)
                                 .ConfigureAwait(false);
        }

        #endregion saved_searches/list

        #region saved_searches/create

        public static async Task<IApiResult<TwitterSavedSearch>> SaveSearchAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] string query,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (query == null) throw new ArgumentNullException(nameof(query));
            var param = new Dictionary<string, object>
            {
                {"query", query}
            };
            return await accessor.PostAsync("saved_searches/create.json", param,
                ResultHandlers.ReadAsSavedSearchAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion saved_searches/create

        #region saved_searches/destroy

        public static async Task<IApiResult<TwitterSavedSearch>> DestroySavedSearchAsync(
            [NotNull] this IApiAccessor accessor, long id,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            return await accessor.PostAsync("saved_searches/destroy/" + id + ".json",
                                     new Dictionary<string, object>(), ResultHandlers.ReadAsSavedSearchAsync,
                                     cancellationToken)
                                 .ConfigureAwait(false);
        }

        #endregion saved_searches/destroy
    }
}