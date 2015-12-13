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
        public IApiAccess Access { get; }

        public long Id { get; }

        public DestroySearchQueryRequest([NotNull] IApiAccess access, long id)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            Access = access;
            Id = id;
        }

        public override Task<IApiResult<TwitterSavedSearch>> Send(CancellationToken token)
        {
            return Access.DestroySavedSearchAsync(Id, token);
        }
    }
}
