using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class UpdateMuteRequest : RequestBase<IApiResult<TwitterUser>>
    {
        [NotNull]
        public IApiAccess Access { get; }

        [NotNull]
        public UserParameter Target { get; }

        public bool Mute { get; }

        public UpdateMuteRequest([NotNull] IApiAccess access, [NotNull] UserParameter target, bool mute)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (target == null) throw new ArgumentNullException(nameof(target));
            Access = access;
            Mute = mute;
            Target = target;
        }

        public override Task<IApiResult<TwitterUser>> Send(CancellationToken token)
        {
            return Access.UpdateMuteAsync(Target, Mute, token);
        }
    }
}
