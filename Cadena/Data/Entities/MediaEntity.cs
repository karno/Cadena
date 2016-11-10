using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena.Data.Entities
{
    public sealed class MediaEntity : TwitterEntity
    {
        public MediaEntity(JsonValue json) : base(json)
        {
            Id = json["id"].AsLong();
            MediaUrl = json["media_url"].AsStringOrNull();
            MediaUrlHttps = json["media_url_https"].AsStringOrNull();
            Url = json["url"].AsStringOrNull();
            DisplayUrl = json["display_url"].AsStringOrNull();
            ExpandedUrl = json["expanded_url"].AsStringOrNull();

            switch (json["type"].AsString())
            {
                case "photo":
                    MediaType = MediaType.Photo;
                    break;
                default:
                    MediaType = MediaType.Unknown;
                    break;
            }

            DisplayText = DisplayUrl ?? String.Empty;
            FullText = ExpandedUrl ?? String.Empty;

            // read video info, if existed

            var vinode = json["video_info"].AsObjectOrNull();
            VideoInfo = vinode != null ? new VideoInfo(vinode) : null;

            // read media sizes
            var medias = new Dictionary<string, MediaSize>();
            var sizes = json["sizes"].AsObjectOrNull();
            if (sizes != null)
            {
                foreach (var size in sizes)
                {
                    medias.Add(size.Key, new MediaSize(size.Value));
                }
            }
            MediaSizes = new ReadOnlyDictionary<string, MediaSize>(medias);
        }

        public override string DisplayText { get; }

        public override string FullText { get; }

        public long Id { get; }

        [CanBeNull]
        public string MediaUrl { get; }

        [CanBeNull]
        public string MediaUrlHttps { get; }

        [CanBeNull]
        public string Url { get; }

        [CanBeNull]
        public string DisplayUrl { get; }

        [CanBeNull]
        public string ExpandedUrl { get; }

        public MediaType MediaType { get; }

        [NotNull]
        public IReadOnlyDictionary<string, MediaSize> MediaSizes { get; }

        [CanBeNull]
        public VideoInfo VideoInfo { get; }
    }

    public enum MediaType
    {
        Unknown = -1,
        Photo
    }

    public struct MediaSize
    {
        public MediaSize(JsonValue json)
        {
            Width = json["w"].AsInteger();
            Height = json["h"].AsInteger();
            switch (json["resize"].AsString())
            {
                case "crop":
                    Resize = MediaResizeMode.Crop;
                    break;
                case "fit":
                    Resize = MediaResizeMode.Fit;
                    break;
                default:
                    Resize = MediaResizeMode.Unknown;
                    break;
            }
        }

        public int Width { get; }

        public int Height { get; }

        public MediaResizeMode Resize { get; }
    }

    public enum MediaResizeMode
    {
        Unknown = -1,
        Crop,
        Fit
    }

    public sealed class VideoInfo
    {
        public VideoInfo(JsonValue videoInfoNode)
        {
            var aspect = videoInfoNode["aspect_ratio"].AsArrayOrNull()?.AsLongArray();
            AspectRatio = aspect == null
                ? Tuple.Create(1, 1)
                : Tuple.Create((int)aspect[0], (int)aspect[1]);
            DurationMillis = videoInfoNode["duration_millis"].AsLong();

            var variants = videoInfoNode["variants"].AsArrayOrNull();
            Variants = variants != null
                ? variants.Select(vnode => new VideoVariant(vnode)).ToArray()
                : new VideoVariant[0];

        }

        [NotNull]
        public Tuple<int, int> AspectRatio { get; }

        public long DurationMillis { get; }

        [NotNull]
        public VideoVariant[] Variants { get; }
    }

    public sealed class VideoVariant
    {
        private static readonly ReadOnlyDictionary<string, VideoContentType> _videoContentTypes =
            new ReadOnlyDictionary<string, VideoContentType>(new Dictionary<string, VideoContentType>
            {
                {"video/mp4", VideoContentType.Mp4 },
                {"video/webm", VideoContentType.WebM },
                {"application/x-mpegURL", VideoContentType.M3U8 },
            });

        public VideoVariant(JsonValue variantNode)
        {
            BitRate = variantNode["bitrate"].AsLong();
            ContentType = variantNode["content_type"].AsString();
            Url = variantNode["url"].AsString();

            // check video content type
            VideoContentType vctype;
            RecognizedContentType = _videoContentTypes.TryGetValue(ContentType, out vctype)
                ? vctype
                : VideoContentType.Unknown;
        }

        public long BitRate { get; }

        public string ContentType { get; }

        public VideoContentType RecognizedContentType { get; }

        public string Url { get; }

    }

    public enum VideoContentType
    {
        Unknown,
        WebM,
        Mp4,
        M3U8
    }
}
