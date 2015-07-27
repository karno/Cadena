using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internal;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena.Api.Streams
{
    public static class UserStreams
    {
        public static async Task Connect([NotNull] IApiAccess access,
            [NotNull] Action<string> parser, TimeSpan readTimeout, CancellationToken cancellationToken,
            [CanBeNull] IEnumerable<string> tracksOrNull = null, bool repliesAll = false,
            bool followingsActivity = false)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            var filteredTracks =
                tracksOrNull != null
                    ? String.Join(",", tracksOrNull.Where(t => !String.IsNullOrEmpty(t.Trim())).Distinct())
                    : null;

            // bulid parameter
            var param = new Dictionary<string, object>
            {
                {"track", String.IsNullOrEmpty(filteredTracks) ? null : filteredTracks},
                {"replies", repliesAll ? "all" : null},
                {"include_followings_activity", followingsActivity ? "true" : null}
            }.ParametalizeForGet();
            var endpoint = HttpUtility.ConcatUrl(access.AccessConfiguration.Endpoint, "user.json");
            if (String.IsNullOrEmpty(param))
            {
                endpoint += "?" + param;
            }

            await Task.Run(async () =>
            {
                HttpClient client = null;
                try
                {
                    // prepare HttpClient
                    client = access.CreateOAuthClient(useGZip: false);
                    // set parameters for receiving UserStreams.
                    client.Timeout = Timeout.InfiniteTimeSpan;
                    client.MaxResponseContentBufferSize = 1024 * 16;
                    // begin connection
                    using (var resp = await client.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken).ConfigureAwait(false))
                    using (var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        // run user stream engine
                        await StreamEngine.Run(stream, parser, readTimeout, cancellationToken);
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
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}
