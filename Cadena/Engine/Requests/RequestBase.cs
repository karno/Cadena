using System.Threading;
using System.Threading.Tasks;

namespace Cadena.Engine.Requests
{
    public abstract class RequestBase : IRequest
    {
        public abstract Task Send(CancellationToken token);
    }

    public abstract class RequestBase<T> : IRequest<T>, IRequest
    {
        Task IRequest.Send(CancellationToken token)
        {
            return Send(token);
        }

        public abstract Task<T> Send(CancellationToken token);
    }
}
