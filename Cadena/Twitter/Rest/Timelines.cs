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
    public static class Timelines
    {
        #region statuses/home_timeline

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> GetHomeTimelineAsync(
            [NotNull] this ApiAccessor accessor,
            int? count, long? sinceId, long? maxId, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"count", count},
                {"since_id", sinceId},
                {"max_id", maxId}
            };
            return await accessor.GetAsync("statuses/home_timeline.json", param,
                ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region statuses/user_timeline

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> GetUserTimelineAsync(
            [NotNull] this ApiAccessor accessor,
            [NotNull] UserParameter targetUser, int? count, long? sinceId, long? maxId,
            bool? excludeReplies, bool? includeRetweets, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (targetUser == null) throw new ArgumentNullException(nameof(targetUser));
            var param = new Dictionary<string, object>
            {
                {"since_id", sinceId},
                {"max_id", maxId},
                {"count", count},
                {"exclude_replies", excludeReplies},
                {"include_rts", includeRetweets},
            }.ApplyParameter(targetUser);
            return await accessor.GetAsync("statuses/user_timeline.json", param,
                ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region statuses/mentions_timeline

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> GetMentionsAsync(
            [NotNull] this ApiAccessor accessor,
            int? count, long? sinceId, long? maxId, bool? includeRetweets,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"count", count},
                {"since_id", sinceId},
                {"max_id", maxId},
                {"include_rts", includeRetweets},
            };
            return await accessor.GetAsync("statuses/mentions_timeline.json", param,
                ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region statuses/retweets_of_me

        public static async Task<IApiResult<IEnumerable<TwitterStatus>>> GetRetweetsOfMeAsync(
            [NotNull] this ApiAccessor accessor,
            int? count, long? sinceId, long? maxId, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"count", count},
                {"since_id", sinceId},
                {"max_id", maxId},
            };
            return await accessor.GetAsync("statuses/retweets_of_me.json", param,
                ResultHandlers.ReadAsStatusCollectionAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
