using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internal;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Api.Rest
{
    public static class Help
    {
        /// <summary>
        /// Get current configuration of Twitter.
        /// </summary>
        /// <param name="access">current configuration of Twitter</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>object represents current configration state of twitter.</returns>
        public static async Task<IApiResult<TwitterConfiguration>> GetConfigurationAsync(
            [NotNull] this IApiAccess access, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            return await access.GetAsync("help/configuration.json",
                new Dictionary<string, object>(), ResultHandlers.ReadAsConfigurationAsync,
                cancellationToken).ConfigureAwait(false);
        }
    }
}
