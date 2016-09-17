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
        public ApiAccessor Accessor { get; }

        [NotNull]
        public UserParameter Target { get; }

        public bool Mute { get; }

        public UpdateMuteRequest([NotNull] ApiAccessor accessor, [NotNull] UserParameter target, bool mute)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (target == null) throw new ArgumentNullException(nameof(target));
            Accessor = accessor;
            Mute = mute;
            Target = target;
        }

        public override Task<IApiResult<TwitterUser>> Send(CancellationToken token)
        {
            return Accessor.UpdateMuteAsync(Target, Mute, token);
        }
    }
}
