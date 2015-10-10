﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers
{
    public class DirectMessagesReceiver : CyclicTimelineReceiverBase
    {
        private readonly IApiAccess _access;
        private readonly Action<Exception> _exceptionHandler;
        private readonly int _receiveCount;

        public DirectMessagesReceiver([NotNull] IApiAccess access, [NotNull] Action<TwitterStatus> handler,
            [NotNull] Action<Exception> exceptionHandler, int receiveCount = 100) : base(handler)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));
            _access = access;
            _exceptionHandler = exceptionHandler;
            _receiveCount = receiveCount;
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            try
            {
                var result = await _access.GetDirectMessagesAsync(_receiveCount,
                    LastSinceId, null, token).ConfigureAwait(false);
                result.Result?.ForEach(CallHandler);
                return result.RateLimit;
            }
            catch (Exception ex)
            {
                _exceptionHandler(ex);
                throw;
            }
        }
    }
}
