using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internal;
using Cadena.Api.Parameters;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Api.Rest
{
    public static class Searching
    {
        #region search/tweets

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> SearchAsync(
            [NotNull] this IApiAccess access, [NotNull] SearchParameter query,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (query == null) throw new ArgumentNullException(nameof(query));

            return await access.GetAsync("search/tweets.json", query.ToDictionary(),
                ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region saved_searches/list

        public static async Task<IApiResult<IEnumerable<TwitterSavedSearch>>> GetSavedSearchesAsync(
            [NotNull] this IApiAccess access, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            return await access.GetAsync("saved_searches/list.json", new Dictionary<string, object>(),
                ResultHandlers.ReadAsSavedSearchCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region saved_searches/create

        public static async Task<IApiResult<TwitterSavedSearch>> SaveSearchAsync(
            [NotNull] this IApiAccess access, [NotNull] string query,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (query == null) throw new ArgumentNullException(nameof(query));
            var param = new Dictionary<string, object>
            {
                {"query", query}
            };
            return await access.PostAsync("saved_searches/create.json", param,
                ResultHandlers.ReadAsSavedSearchAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region saved_searches/destroy

        public static async Task<IApiResult<TwitterSavedSearch>> DestroySavedSearchAsync(
            [NotNull] this IApiAccess access, long id,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            return await access.PostAsync("saved_searches/destroy/" + id + ".json",
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
