using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class RetweetRequest : RequestBase<IApiResult<TwitterStatus>>
    {
        [NotNull]
        public IApiAccessor Accessor { get; }

        public long TargetId { get; }

        public RetweetRequest([NotNull] IApiAccessor accessor, long targetId)
        {
            Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            TargetId = targetId;
        }

        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            return Accessor.RetweetAsync(TargetId, token);
        }
    }
}