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
    public static class DirectMessages
    {
        #region direct_messages

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> GetDirectMessagesAsync(
            [NotNull] this IApiAccess access, int? count, long? sinceId, long? maxId,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object>
            {
                {"count", count},
                {"since_id", sinceId},
                {"max_id", maxId},
            };
            return await access.GetAsync("direct_messages.json", param,
                ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region direct_messages/sent

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> GetSentDirectMessagesAsync(
            [NotNull] this IApiAccess access,
            int? count, long? sinceId, long? maxId, int? page, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object>
            {
                {"count", count},
                {"since_id", sinceId},
                {"max_id", maxId},
                {"page", page},
            };
            return await access.GetAsync("direct_messages/sent.json", param,
                ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region direct_messages/show

        public static async Task<IApiResult<TwitterStatus>> ShowDirectMessageAsync(
            [NotNull] this IApiAccess access, long id,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object>
            {
                {"id", id}
            };
            return await access.GetAsync("direct_messages/show.json", param,
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region direct_messages/new

        public static async Task<IApiResult<TwitterStatus>> SendDirectMessageAsync(
            [NotNull] this IApiAccess access, [NotNull] UserParameter recipient, [NotNull] string text,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (recipient == null) throw new ArgumentNullException(nameof(recipient));
            if (text == null) throw new ArgumentNullException(nameof(text));
            var param = new Dictionary<string, object>
            {
                {"text", text}
            }.ApplyParameter(recipient);
            return await access.PostAsync("direct_messages/new.json", param,
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region direct_messages/destroy

        public static async Task<IApiResult<TwitterStatus>> DestroyDirectMessageAsync(
            [NotNull] this IApiAccess access, long id,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object>
            {
                {"id", id}
            };
            return await access.PostAsync("direct_messages/destroy.json", param,
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
