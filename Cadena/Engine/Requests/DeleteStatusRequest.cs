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
        public ApiAccessor Accessor { get; }

        public long Id { get; }

        public StatusType Type { get; }

        public DeleteStatusRequest([NotNull] ApiAccessor accessor, long id, StatusType type)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            Accessor = accessor;
            Id = id;
            Type = type;
        }

        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            return Type == StatusType.Tweet
                ? Accessor.DestroyAsync(Id, token)
                : Accessor.DestroyDirectMessageAsync(Id, token);
        }
    }
}
