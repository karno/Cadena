using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class DeleteStatusRequest : RequestBase<IApiResult<TwitterStatus>>
    {
        [NotNull]
        public IApiAccess Access { get; }

        public long Id { get; }

        public StatusType Type { get; }

        public DeleteStatusRequest([NotNull] IApiAccess access, long id, StatusType type)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            Access = access;
            Id = id;
            Type = type;
        }

        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            return Type == StatusType.Tweet
                ? Access.DestroyAsync(Id, token)
                : Access.DestroyDirectMessageAsync(Id, token);
        }
    }
}
