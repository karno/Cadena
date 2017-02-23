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
    public static class Blocks
    {
        #region blocks/ids

        public static async Task<IApiResult<ICursorResult<IEnumerable<long>>>> GetBlocksIdsAsync(
            [NotNull] this IApiAccessor accessor, long cursor, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object> { { "cursor", cursor } };
            return await accessor.GetAsync("blocks/ids.json", param,
                ResultHandlers.ReadAsCursoredIdsAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region blocks/create

        public static async Task<IApiResult<TwitterUser>> CreateBlockAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] UserParameter targetUser,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            return await accessor.PostAsync("blocks/create.json", targetUser.ToDictionary(),
                ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region blocks/destroy

        public static async Task<IApiResult<TwitterUser>> DestroyBlockAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] UserParameter targetUser,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            return await accessor.PostAsync("blocks/destroy.json", targetUser.ToDictionary(),
                ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region users/report_spam

        public static async Task<IApiResult<TwitterUser>> ReportSpamAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] UserParameter targetUser,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            return await accessor.PostAsync("users/report_spam.json", targetUser.ToDictionary(),
                ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
