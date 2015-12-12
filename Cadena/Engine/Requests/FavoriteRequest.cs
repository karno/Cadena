using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;

namespace Cadena.Engine.Requests
{
    public class FavoriteRequest : RequestBase<IApiResult<TwitterStatus>>
    {
        private readonly IApiAccess _access;
        private readonly long _targetTweetId;
        private readonly bool _createFavorite;

        public FavoriteRequest(IApiAccess access, TwitterStatus status, bool createFavorite)
            : this(access, status.RetweetedStatusId ?? status.Id, createFavorite)
        {
        }

        public FavoriteRequest(IApiAccess access, long targetTweetId, bool createFavorite)
        {
            _access = access;
            _targetTweetId = targetTweetId;
            _createFavorite = createFavorite;
        }

        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            return _createFavorite
                ? _access.CreateFavoriteAsync(_targetTweetId, token)
                : _access.DestroyFavoriteAsync(_targetTweetId, token);
        }
    }
}
