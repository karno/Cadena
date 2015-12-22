using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internals;
using Cadena.Util;
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
        public static async Task ConnectAsync([NotNull] ApiAccessor accessor,
            [NotNull] Action<string> parser, TimeSpan readTimeout, CancellationToken cancellationToken,
            [CanBeNull] IEnumerable<string> tracksOrNull = null, bool stallWarnings = false,
            StreamFilterLevel filterLevel = StreamFilterLevel.None,
            bool repliesAll = false, bool followingsActivity = false)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            // remove empty string and remove duplicates, concat strings
            var filteredTracks = tracksOrNull?.Select(t => t?.Trim())
                                              .Where(t => !String.IsNullOrEmpty(t))
                                              .Distinct()
                                              .JoinString(",");
            // bulid parameter
            var param = new Dictionary<string, object>
            {
                {"track", String.IsNullOrEmpty(filteredTracks) ? null : filteredTracks},
                {"stall_warnings", stallWarnings ? "true" : null},
                {"filter_level", filterLevel == StreamFilterLevel.None ? null : filterLevel.ToParamString()},
                {"replies", repliesAll ? "all" : null},
                {"include_followings_activity", followingsActivity ? "true" : null}
            }.ParametalizeForGet();
            var endpoint = HttpUtility.ConcatUrl(accessor.Endpoint, "user.json");

            // join parameters to endpoint URL
            if (!String.IsNullOrEmpty(param))
            {
                endpoint += "?" + param;
            }

            // begin connection
            HttpClient client = null;
            try
            {
                // prepare HttpClient
                client = accessor.GetClientForStreaming();
                // set parameters for receiving UserStreams.
                client.Timeout = Timeout.InfiniteTimeSpan;
                // begin connection
                using (var resp = await client.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken).ConfigureAwait(false))
                using (var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    // winding data from user stream
                    await StreamWinder.Run(stream, parser, readTimeout, cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                if (client != null)
                {
                    // cancel pending requests
                    client.CancelPendingRequests();
                    client.Dispose();
                }
            }
        }
    }
}
