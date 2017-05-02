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
        public IApiAccessor Accessor { get; }

        [NotNull]
        public UserParameter Target { get; }

        public bool Mute { get; }

        public UpdateMuteRequest([NotNull] IApiAccessor accessor, [NotNull] UserParameter target, bool mute)
        {
            Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            Mute = mute;
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public override Task<IApiResult<TwitterUser>> Send(CancellationToken token)
        {
            return Accessor.UpdateMuteAsync(Target, Mute, token);
        }
    }
}