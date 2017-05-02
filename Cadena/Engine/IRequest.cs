using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Cadena.Engine
{
    public interface IRequest
    {
        [NotNull]
        Task Send(CancellationToken token);
    }

    public interface IRequest<T>
    {
        [NotNull]
        Task<T> Send(CancellationToken token);
    }
}