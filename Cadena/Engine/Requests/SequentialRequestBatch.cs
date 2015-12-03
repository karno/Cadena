using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cadena.Engine.Requests
{
    public sealed class SequentialRequestBatch : IRequest, IEnumerable<IRequest>
    {
        private Queue<IRequest> _requestQueue;

        public SequentialRequestBatch()
        {
            _requestQueue = new Queue<IRequest>();
        }

        public IEnumerator<IRequest> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Task Send(CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
