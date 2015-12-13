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
        public IApiAccess Access { get; }

        public long TargetTweetId { get; }

        public bool CreateFavorite { get; }

        public FavoriteRequest([NotNull] IApiAccess access, long targetTweetId, bool createFavorite)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            Access = access;
            TargetTweetId = targetTweetId;
            CreateFavorite = createFavorite;
        }

        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            return CreateFavorite
                ? Access.CreateFavoriteAsync(TargetTweetId, token)
                : Access.DestroyFavoriteAsync(TargetTweetId, token);
        }
    }
}
