using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class FavoriteRequest : RequestBase<IApiResult<TwitterStatus>>
    {
        [NotNull]
        public IApiAccessor Accessor { get; }

        public long TargetTweetId { get; }

        public bool CreateFavorite { get; }

        public FavoriteRequest([NotNull] IApiAccessor accessor, long targetTweetId, bool createFavorite)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            Accessor = accessor;
            TargetTweetId = targetTweetId;
            CreateFavorite = createFavorite;
        }

        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            return CreateFavorite
                ? Accessor.CreateFavoriteAsync(TargetTweetId, token)
                : Accessor.DestroyFavoriteAsync(TargetTweetId, token);
        }
    }
}
