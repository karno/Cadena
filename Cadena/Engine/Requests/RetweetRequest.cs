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
        public IApiAccess Access { get; }

        public long TargetId { get; }

        public RetweetRequest([NotNull] IApiAccess access, long targetId)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            Access = access;
            TargetId = targetId;
        }

        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            return Access.RetweetAsync(TargetId, token);
        }
    }
}
