using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Cadena.Twitter.Parameters
{
    public sealed class StatusParameter : ParameterBase
    {
        private string _status;

        [NotNull]
        public string Status
        {
            get { return _status; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _status = value;
            }
        }

        public long? InReplyToStatusId { get; }

        public bool? PossiblySensitive { get; }

        public Tuple<double, double> GeoLatLong { get; }

        public string PlaceId { get; }

        public bool? DisplayCoordinates { get; }

        public long[] MediaIds { get; }

        public StatusParameter([NotNull] string status, long? inReplyToStatusId = null,
            bool? possiblySensitive = null, [CanBeNull] Tuple<double, double> geoLatLong = null,
            [CanBeNull] string placeId = null, bool? displayCoordinates = null,
            [CanBeNull] long[] mediaIds = null)
        {
            if (status == null) throw new ArgumentNullException(nameof(status));
            _status = status;
            InReplyToStatusId = inReplyToStatusId;
            PossiblySensitive = possiblySensitive;
            GeoLatLong = geoLatLong;
            PlaceId = placeId;
            DisplayCoordinates = displayCoordinates;
            MediaIds = mediaIds;
        }

        public override void SetDictionary(IDictionary<string, object> target)
        {
            var mediaIdStr = MediaIds != null
                ? String.Join(",", MediaIds.Select(s => s.ToString()))
                : null;
            target["status"] = _status;
            target["in_reply_to_status_id"] = InReplyToStatusId;
            target["possibly_sensitive"] = PossiblySensitive;
            target["lat"] = GeoLatLong?.Item1;
            target["long"] = GeoLatLong?.Item2;
            target["place_id"] = PlaceId;
            target["display_coordinates"] = DisplayCoordinates;
            target["media_ids"] = mediaIdStr;
        }
    }
}
