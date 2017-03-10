using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internals;
using JetBrains.Annotations;

namespace Cadena.Api.Streams
{
    public static class UserStreams
    {
        /// <summary>
        /// Connect to user streams.
        /// </summary>
        /// <param name="accessor">API access preference</param>
        /// <param name="parser">Line handler</param>
        /// <param name="readTimeout">stream read timeout</param>
        /// <param name="cancellationToken">cancellation token object</param>
        /// <param name="tracksOrNull">tracks parameter(can be null)</param>
        /// <param name="stallWarnings">request stall warnings</param>
        /// <param name="filterLevel">stream filtering level</param>
        /// <param name="repliesAll">repliesAll parameter</param>
        /// <param name="followingsActivity">include_followings_activity parameter</param>
        /// <returns></returns>
        public static Task ConnectAsync([NotNull] IApiAccessor accessor,
            [NotNull] Action<string> parser, TimeSpan readTimeout, CancellationToken cancellationToken,
            [CanBeNull] IEnumerable<string> tracksOrNull = null, bool stallWarnings = false,
            StreamFilterLevel filterLevel = StreamFilterLevel.None,
            bool repliesAll = false, bool followingsActivity = false)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            // remove empty string and remove duplicates, concatenate strings
            var filteredTracks = tracksOrNull?.Select(t => t?.Trim())
                                              .Where(t => !String.IsNullOrEmpty(t))
                                              .Distinct()
                                              .JoinString(",");
            // build parameter
            var param = new Dictionary<string, object>
            {
                {"track", String.IsNullOrEmpty(filteredTracks) ? null : filteredTracks},
                {"stall_warnings", stallWarnings ? "true" : null},
                {"filter_level", filterLevel == StreamFilterLevel.None ? null : filterLevel.ToParamString()},
                {"replies", repliesAll ? "all" : null},
                {"include_followings_activity", followingsActivity ? "true" : null}
            };

            // begin connection
            return accessor.ConnectStreamAsync("user.json", param,
                stream => StreamWinder.Run(stream, parser, readTimeout, cancellationToken),
                cancellationToken);
        }
    }
}
