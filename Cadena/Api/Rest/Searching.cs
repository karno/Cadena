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
    public static class Searching
    {
        #region search/tweets

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> SearchAsync(
            [NotNull] this ApiAccessor accessor, [NotNull] SearchParameter query,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (query == null) throw new ArgumentNullException(nameof(query));

            return await accessor.GetAsync("search/tweets.json", query.ToDictionary().SetExtended(),
                ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region saved_searches/list

        public static async Task<IApiResult<IEnumerable<TwitterSavedSearch>>> GetSavedSearchesAsync(
            [NotNull] this ApiAccessor accessor, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            return await accessor.GetAsync("saved_searches/list.json", new Dictionary<string, object>(),
                ResultHandlers.ReadAsSavedSearchCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region saved_searches/create

        public static async Task<IApiResult<TwitterSavedSearch>> SaveSearchAsync(
            [NotNull] this ApiAccessor accessor, [NotNull] string query,
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

        #endregion

        #region saved_searches/destroy

        public static async Task<IApiResult<TwitterSavedSearch>> DestroySavedSearchAsync(
            [NotNull] this ApiAccessor accessor, long id,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            return await accessor.PostAsync("saved_searches/destroy/" + id + ".json",
                new Dictionary<string, object>(), ResultHandlers.ReadAsSavedSearchAsync, cancellationToken)
                                   .ConfigureAwait(false);
        }

        #endregion
    }

    public enum SearchResultType
    {
        Mixed,
        Recent,
        Popular,
    }
}
