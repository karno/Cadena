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
        public ApiAccessor Accessor { get; }

        public long TargetId { get; }

        public RetweetRequest([NotNull] ApiAccessor accessor, long targetId)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            Accessor = accessor;
            TargetId = targetId;
        }

        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            return Accessor.RetweetAsync(TargetId, token);
        }
    }
}
