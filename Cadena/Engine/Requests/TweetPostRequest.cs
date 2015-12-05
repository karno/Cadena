using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;

namespace Cadena.Engine.Requests
{
    public class TweetPostRequest : RequestBase<IApiResult<TwitterStatus>>
    {
        private readonly IApiAccess _access;
        private readonly StatusParameter _parameter;

        public TweetPostRequest(IApiAccess access, StatusParameter parameter)
        {
            _access = access;
            _parameter = parameter;
        }

        public override async Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            return await _access.UpdateAsync(_parameter, token);
        }
    }
}
