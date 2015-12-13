using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class SaveSearchQueryRequest : RequestBase<IApiResult<TwitterSavedSearch>>
    {
        [NotNull]
        public IApiAccess Access { get; }

        [NotNull]
        public string Query { get; }

        public SaveSearchQueryRequest([NotNull] IApiAccess access, [NotNull] string query)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (String.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Search query must not be empty or whitespace only.");
            }
            Access = access;
            Query = query;
        }

        public override Task<IApiResult<TwitterSavedSearch>> Send(CancellationToken token)
        {
            return Access.SaveSearchAsync(Query, token);
        }
    }
}
