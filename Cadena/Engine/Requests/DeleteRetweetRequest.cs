using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class DeleteRetweetRequest : RequestBase<IApiResult<TwitterStatus>>
    {
        [NotNull]
        public IApiAccessor Accessor { get; }

        public long TargetId { get; }

        public DeleteRetweetRequest([NotNull] IApiAccessor accessor, long targetId)
        {
            Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            TargetId = targetId;
        }

        public override async Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            var r = await Accessor.GetMyRetweetIdOfStatusAsync(TargetId, token).ConfigureAwait(false);
            if (!r.Result.HasValue)
            {
                // retweet is not existed.
                return null;
            }
            // destroy retweet
            return await Accessor.DestroyAsync(r.Result.Value, token).ConfigureAwait(false);
        }
    }
}