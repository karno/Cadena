﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Timelines
{
    public class DirectMessagesReceiver : CyclicTimelineReceiverBase
    {
        private readonly ApiAccessor _accessor;
        private readonly int _receiveCount;

        public DirectMessagesReceiver([NotNull] ApiAccessor accessor, [NotNull] Action<TwitterStatus> handler,
            [CanBeNull] Action<Exception> exceptionHandler, int receiveCount = 100) : base(handler, exceptionHandler)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            _accessor = accessor;
            _receiveCount = receiveCount;
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            try
            {
                var result = await _accessor.GetDirectMessagesAsync(_receiveCount,
                    LastSinceId, null, token).ConfigureAwait(false);
                result.Result?.ForEach(CallHandler);
                return result.RateLimit;
            }
            catch (Exception ex)
            {
                CallExceptionHandler(ex);
                throw;
            }
        }
    }
}