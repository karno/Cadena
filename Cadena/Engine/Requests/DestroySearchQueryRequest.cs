using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class DestroySearchQueryRequest : RequestBase<IApiResult<TwitterSavedSearch>>
    {
        [NotNull]
        public IApiAccessor Accessor { get; }

        public long Id { get; }

        public DestroySearchQueryRequest([NotNull] IApiAccessor accessor, long id)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            Accessor = accessor;
            Id = id;
        }

        public override Task<IApiResult<TwitterSavedSearch>> Send(CancellationToken token)
        {
            return Accessor.DestroySavedSearchAsync(Id, token);
        }
    }
}
