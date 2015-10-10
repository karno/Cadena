using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Data;

namespace Cadena.Engine.CyclicReceivers
{
    public class SearchReceiver : CyclicReceiverBase
    {
        protected override Task<RateLimitDescription> Execute(CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
