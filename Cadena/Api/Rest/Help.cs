using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Data;
using Cadena._Internals;
using JetBrains.Annotations;

namespace Cadena.Api.Rest
{
    public static class Help
    {
        /// <summary>
        /// Get current configuration of Twitter.
        /// </summary>
        /// <param name="accessor">current configuration of Twitter</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>object represents current configuration state of twitter.</returns>
        public static async Task<IApiResult<TwitterConfiguration>> GetConfigurationAsync(
            [NotNull] this IApiAccessor accessor, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            return await accessor.GetAsync("help/configuration.json",
                new Dictionary<string, object>(), ResultHandlers.ReadAsConfigurationAsync,
                cancellationToken).ConfigureAwait(false);
        }
    }
}