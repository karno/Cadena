using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internals;
using Cadena.Api.Parameters;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Api.Rest
{
    public static class Blockings
    {
        #region blocks/ids

        public static async Task<IApiResult<ICursorResult<IEnumerable<long>>>> GetBlockingsIdsAsync(
            [NotNull] this IApiAccess access, long cursor, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object> { { "cursor", cursor } };
            return await access.GetAsync("blocks/ids.json", param,
                ResultHandlers.ReadAsCursoredIdsAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region blocks/create

        public static async Task<IApiResult<TwitterUser>> CreateBlockAsync(
            [NotNull] this IApiAccess access, [NotNull] UserParameter targetUser,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            return await access.PostAsync("blocks/create.json", targetUser.ToDictionary(),
                ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region blocks/destroy

        public static async Task<IApiResult<TwitterUser>> DestroyBlockAsync(
            [NotNull] this IApiAccess access, [NotNull] UserParameter targetUser,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            return await access.PostAsync("blocks/destroy.json", targetUser.ToDictionary(),
                ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region users/report_spam

        public static async Task<IApiResult<TwitterUser>> ReportSpamAsync(
            [NotNull] this IApiAccess access, [NotNull] UserParameter targetUser,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            return await access.PostAsync("users/report_spam.json", targetUser.ToDictionary(),
                ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
