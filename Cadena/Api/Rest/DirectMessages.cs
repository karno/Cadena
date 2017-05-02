using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Data;
using Cadena._Internals;
using JetBrains.Annotations;

namespace Cadena.Api.Rest
{
    public static class DirectMessages
    {
        #region direct_messages

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> GetDirectMessagesAsync(
            [NotNull] this IApiAccessor accessor, int? count, long? sinceId, long? maxId,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"count", count},
                {"since_id", sinceId},
                {"max_id", maxId},
                {"full_text", true} // full_text mode is always applied
            }.SetExtended();
            return await accessor.GetAsync("direct_messages.json", param,
                                     ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken)
                                 .ConfigureAwait(false);
        }

        #endregion direct_messages

        #region direct_messages/sent

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> GetSentDirectMessagesAsync(
            [NotNull] this IApiAccessor accessor,
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
            }.SetExtended();
            return await accessor.GetAsync("direct_messages/sent.json", param,
                                     ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken)
                                 .ConfigureAwait(false);
        }

        #endregion direct_messages/sent

        #region direct_messages/show

        public static async Task<IApiResult<TwitterStatus>> ShowDirectMessageAsync(
            [NotNull] this IApiAccessor accessor, long id,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"id", id},
                {"full_text", true} // full_text mode is always applied
            }.SetExtended();
            return await accessor.GetAsync("direct_messages/show.json", param,
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion direct_messages/show

        #region direct_messages/new

        public static async Task<IApiResult<TwitterStatus>> SendDirectMessageAsync(
            [NotNull] this IApiAccessor accessor, [NotNull] UserParameter recipient, [NotNull] string text,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (recipient == null) throw new ArgumentNullException(nameof(recipient));
            if (text == null) throw new ArgumentNullException(nameof(text));
            var param = new Dictionary<string, object>
            {
                {"text", text}
            }.ApplyParameter(recipient).SetExtended();
            return await accessor.PostAsync("direct_messages/new.json", param,
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion direct_messages/new

        #region direct_messages/destroy

        public static async Task<IApiResult<TwitterStatus>> DestroyDirectMessageAsync(
            [NotNull] this IApiAccessor accessor, long id,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"id", id}
            }.SetExtended();
            return await accessor.PostAsync("direct_messages/destroy.json", param,
                ResultHandlers.ReadAsStatusAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion direct_messages/destroy
    }
}