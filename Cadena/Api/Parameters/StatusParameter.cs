using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Cadena.Api.Parameters
{
    public sealed class StatusParameter : ParameterBase
    {
        [NotNull]
        public string Status { get; }

        public long? InReplyToStatusId { get; }

        public bool AutoPopulateReplyMetadata { get; }

        public bool? PossiblySensitive { get; }

        public Tuple<double, double> GeoLatLong { get; }

        public string PlaceId { get; }

        public string AttachmentUrl { get; }

        public bool? DisplayCoordinates { get; }

        public long[] MediaIds { get; }

        public StatusParameter([NotNull] string status, long? inReplyToStatusId = null,
            bool? possiblySensitive = null, [CanBeNull] Tuple<double, double> geoLatLong = null,
            [CanBeNull] string placeId = null, bool? displayCoordinates = null,
            [CanBeNull] long[] mediaIds = null, [CanBeNull] string attachmentUrl = null)
        {
            Status = status ?? throw new ArgumentNullException(nameof(status));
            InReplyToStatusId = inReplyToStatusId;
            AutoPopulateReplyMetadata = inReplyToStatusId != null;
            PossiblySensitive = possiblySensitive;
            GeoLatLong = geoLatLong;
            PlaceId = placeId;
            DisplayCoordinates = displayCoordinates;
            MediaIds = mediaIds;
            AttachmentUrl = attachmentUrl;
        }

        public override void SetDictionary(IDictionary<string, object> target)
        {
            var mediaIdStr = MediaIds != null
                ? String.Join(",", MediaIds.Select(s => s.ToString()))
                : null;
            target["status"] = Status;
            target["in_reply_to_status_id"] = InReplyToStatusId;
            target["auto_populate_reply_metadata"] = AutoPopulateReplyMetadata;
            target["possibly_sensitive"] = PossiblySensitive;
            target["lat"] = GeoLatLong?.Item1;
            target["long"] = GeoLatLong?.Item2;
            target["place_id"] = PlaceId;
            target["display_coordinates"] = DisplayCoordinates;
            target["media_ids"] = mediaIdStr;
            target["attachment_url"] = AttachmentUrl;
        }
    }
}