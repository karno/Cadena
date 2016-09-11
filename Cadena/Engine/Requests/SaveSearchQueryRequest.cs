using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Data;
using Cadena.Twitter.Rest;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class SaveSearchQueryRequest : RequestBase<IApiResult<TwitterSavedSearch>>
    {
        [NotNull]
        public ApiAccessor Accessor { get; }

        [NotNull]
        public string Query { get; }

        public SaveSearchQueryRequest([NotNull] ApiAccessor accessor, [NotNull] string query)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (String.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Search query must not be empty or whitespace only.");
            }
            Accessor = accessor;
            Query = query;
        }

        public override Task<IApiResult<TwitterSavedSearch>> Send(CancellationToken token)
        {
            return Accessor.SaveSearchAsync(Query, token);
        }
    }
}
