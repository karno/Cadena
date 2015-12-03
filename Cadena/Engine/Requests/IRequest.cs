using System.Threading;
using System.Threading.Tasks;

namespace Cadena.Engine.Requests
{
    public interface IRequest
    {
        Task Send(CancellationToken token);
    }
}
