using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internals;
using Cadena.Data;
using Cadena.Twitter.Parameters;
using JetBrains.Annotations;

namespace Cadena.Twitter.Rest
{
    public static class DirectMessages
    {
        #region direct_messages

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> GetDirectMessagesAsync(
            [NotNull] this ApiAccessor accessor, int? count, long? sinceId, long? maxId,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"count", count},
                {"since_id", sinceId},
                {"max_id", maxId},
                {"full_text", true} // full_text mode is always applied
            };
            return await accessor.GetAsync("direct_messages.json", param,
                ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region direct_messages/sent

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> GetSentDirectMessagesAsync(
            [NotNull] this ApiAccessor accessor,
            int? count, long? sinceId, long? maxId, int? page, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"count", count},
                {"since_id", sinceId},
                {"max_id", maxId},
                {"page", page},
                {"full_text", true} // full_text mode is always applied
            };
            return await accessor.GetAsync("direct_messages/sent.json", param,
                ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region direct_messages/show

        public static async Task<IApiResult<TwitterStatus>> ShowDirectMessageAsync(
            [NotNull] this ApiAccessor accessor, long id,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"id", id},
                {"full_text", true} // full_text mode is always applied
            };
            return await accessor.GetAsync("direct_messages/show.json", param,
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region direct_messages/new

        public static async Task<IApiResult<TwitterStatus>> SendDirectMessageAsync(
            [NotNull] this ApiAccessor accessor, [NotNull] UserParameter recipient, [NotNull] string text,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (recipient == null) throw new ArgumentNullException(nameof(recipient));
            if (text == null) throw new ArgumentNullException(nameof(text));
            var param = new Dictionary<string, object>
            {
                {"text", text}
            }.ApplyParameter(recipient);
            return await accessor.PostAsync("direct_messages/new.json", param,
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region direct_messages/destroy

        public static async Task<IApiResult<TwitterStatus>> DestroyDirectMessageAsync(
            [NotNull] this ApiAccessor accessor, long id,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"id", id}
            };
            return await accessor.PostAsync("direct_messages/destroy.json", param,
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
