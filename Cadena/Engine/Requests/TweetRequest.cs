using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class TweetRequest : RequestBase<IApiResult<TwitterStatus>>
    {
        public IApiAccess Access { get; }

        public string Text { get; }

        public long? InReplyToStatusId { get; }

        public Tuple<double, double> GeoLatLong { get; }

        public string PlaceId { get; }

        public bool? DisplayCoordinates { get; }

        public TweetRequest(IApiAccess access, [NotNull] string text, long? inReplyToStatusId = null,
            [CanBeNull] Tuple<double, double> geoLatLong = null,
            [CanBeNull] string placeId = null, bool? displayCoordinates = null)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Access = access;
            Text = text;
            InReplyToStatusId = inReplyToStatusId;
            GeoLatLong = geoLatLong;
            PlaceId = placeId;
            DisplayCoordinates = displayCoordinates;
        }


        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            var param = new StatusParameter(Text, InReplyToStatusId, null, GeoLatLong, PlaceId, DisplayCoordinates);
            return Access.UpdateAsync(param, token);
        }
    }

    public class TweetWithMediaRequest : TweetRequest
    {
        public long[] MediaIds { get; }

        public bool PossiblySensitive { get; }

        public TweetWithMediaRequest(IApiAccess access, [NotNull] string text, IEnumerable<long> mediaIds,
            bool possiblySensitive, long? inReplyToStatusId = null,
            [CanBeNull] Tuple<double, double> geoLatLong = null, [CanBeNull] string placeId = null,
            bool? displayCoordinates = null)
            : base(access, text, inReplyToStatusId, geoLatLong, placeId, displayCoordinates)
        {
            MediaIds = mediaIds.ToArray();
            PossiblySensitive = possiblySensitive;
        }

        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            var param = new StatusParameter(Text, InReplyToStatusId, PossiblySensitive,
                GeoLatLong, PlaceId, DisplayCoordinates, MediaIds);
            return Access.UpdateAsync(param, token);
        }
    }
}
