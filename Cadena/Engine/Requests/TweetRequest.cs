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
        [NotNull]
        public ApiAccessor Accessor { get; }

        [NotNull]
        public string Text { get; }

        public long? InReplyToStatusId { get; }

        [CanBeNull]
        public Tuple<double, double> GeoLatLong { get; }

        [CanBeNull]
        public string PlaceId { get; }

        public bool? DisplayCoordinates { get; }

        public TweetRequest([NotNull] ApiAccessor accessor, [NotNull] string text,
            long? inReplyToStatusId = null, [CanBeNull] Tuple<double, double> geoLatLong = null,
            [CanBeNull] string placeId = null, bool? displayCoordinates = null)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (text == null) throw new ArgumentNullException(nameof(text));
            Accessor = accessor;
            Text = text;
            InReplyToStatusId = inReplyToStatusId;
            GeoLatLong = geoLatLong;
            PlaceId = placeId;
            DisplayCoordinates = displayCoordinates;
        }

        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            var param = new StatusParameter(Text, InReplyToStatusId, null, GeoLatLong, PlaceId, DisplayCoordinates);
            return Accessor.UpdateAsync(param, token);
        }
    }

    public class TweetWithMediaRequest : TweetRequest
    {
        [NotNull]
        public long[] MediaIds { get; }

        public bool PossiblySensitive { get; }

        public TweetWithMediaRequest([NotNull] ApiAccessor accessor, [NotNull] string text,
            IEnumerable<long> mediaIds, bool possiblySensitive, long? inReplyToStatusId = null,
            [CanBeNull] Tuple<double, double> geoLatLong = null, [CanBeNull] string placeId = null,
            bool? displayCoordinates = null)
            : base(accessor, text, inReplyToStatusId, geoLatLong, placeId, displayCoordinates)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            MediaIds = mediaIds.ToArray();
            PossiblySensitive = possiblySensitive;
        }

        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            var param = new StatusParameter(Text, InReplyToStatusId, PossiblySensitive,
                GeoLatLong, PlaceId, DisplayCoordinates, MediaIds);
            return Accessor.UpdateAsync(param, token);
        }
    }
}
