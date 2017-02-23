﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers
{
    public class UserInfoReceiver : CyclicReceiverBase
    {
        private readonly IApiAccessor _accessor;
        private readonly Action<TwitterUser> _handler;
        private readonly UserParameter _target;

        public UserInfoReceiver(IApiAccessor accessor, [NotNull] Action<TwitterUser> handler,
            [CanBeNull] Action<Exception> exceptionHandler, [NotNull] UserParameter target)
            : base(exceptionHandler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (target == null) throw new ArgumentNullException(nameof(target));
            _accessor = accessor;
            _handler = handler;
            _target = target;
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            try
            {
                var result = await _accessor.ShowUserAsync(_target, token).ConfigureAwait(false);
                _handler(result.Result);
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
