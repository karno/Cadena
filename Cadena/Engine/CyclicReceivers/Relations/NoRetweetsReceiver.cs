﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Relations
{
    public class NoRetweetsReceiver : CyclicRelationInfoReceiverBase
    {
        private readonly IApiAccessor _accessor;

        public NoRetweetsReceiver([NotNull] IApiAccessor accessor, [NotNull] Action<IEnumerable<long>> handler,
            [CanBeNull] Action<Exception> exceptionHandler) : base(handler, exceptionHandler)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            var result = await _accessor.GetNoRetweetsIdsAsync(token).ConfigureAwait(false);
            CallHandler(result.Result);
            return result.RateLimit;
        }
    }
}