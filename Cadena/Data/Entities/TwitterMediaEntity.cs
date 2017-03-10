using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena.Data.Entities
{
    public sealed class TwitterMediaEntity : TwitterEntity
    {
        public TwitterMediaEntity(JsonValue json) : base(json)
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
                case "video":
                    MediaType = MediaType.Video;
                    break;
                case "animated_gif":
                    MediaType = MediaType.AnimatedGif;
                    break;
                default:
                    MediaType = MediaType.Unknown;
                    break;
            }

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

        public TwitterMediaEntity(Tuple<int, int> indices,
            long id, [CanBeNull] string mediaUrl, [CanBeNull] string mediaUrlHttps,
            [CanBeNull] string url, [CanBeNull] string displayUrl, [CanBeNull] string expandedUrl,
            MediaType mediaType, [NotNull] IReadOnlyDictionary<string, MediaSize> mediaSizes,
            [CanBeNull] VideoInfo videoInfo)
            : base(indices)
        {
            if (mediaSizes == null) throw new ArgumentNullException(nameof(mediaSizes));
            Id = id;
            MediaUrl = mediaUrl;
            MediaUrlHttps = mediaUrlHttps;
            Url = url;
            DisplayUrl = displayUrl;
            ExpandedUrl = expandedUrl;
            MediaType = mediaType;
            MediaSizes = mediaSizes;
            VideoInfo = videoInfo;
        }

        public long Id { get; }

        public override string DisplayText => DisplayUrl ?? String.Empty;

        public override string FullText => ExpandedUrl ?? String.Empty;

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
        Photo,
        Video,
        AnimatedGif,
    }

    public struct MediaSize
    {
        private MediaSize(string resizeString)
        {
            Width = 0;
            Height = 0;
            switch (resizeString)
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
            ResizeString = resizeString;
        }

        public MediaSize(JsonValue json) : this(json["resize"].AsString())
        {
            Width = json["w"].AsInteger();
            Height = json["h"].AsInteger();
        }

        public MediaSize(int width, int height, string resizeString) : this(resizeString)
        {
            Width = width;
            Height = height;
        }

        public MediaSize(int width, int height, MediaResizeMode resize)
        {
            Width = width;
            Height = height;
            ResizeString = resize.ToString();
            Resize = resize;
        }

        public int Width { get; }

        public int Height { get; }

        [NotNull]
        public string ResizeString { get; }

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

        public VideoInfo(Tuple<int, int> aspectRatio, long durationMillis, VideoVariant[] variants)
        {
            AspectRatio = aspectRatio;
            DurationMillis = durationMillis;
            Variants = variants;
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
                {"video/mp4", VideoContentType.Mp4},
                {"video/webm", VideoContentType.WebM},
                {"application/x-mpegURL", VideoContentType.M3U8},
            });

        public VideoVariant(JsonValue variantNode)
        {
            BitRate = variantNode["bitrate"].AsLong();
            ContentType = variantNode["content_type"].AsString();
            Url = variantNode["url"].AsString();

            // check video content type
            RecognizedContentType = _videoContentTypes.TryGetValue(ContentType, out var vctype)
                ? vctype
                : VideoContentType.Unknown;
        }

        public VideoVariant(long bitRate, string contentType, string url,
            VideoContentType recognizedContentType)
        {
            BitRate = bitRate;
            ContentType = contentType;
            Url = url;
            RecognizedContentType = recognizedContentType;
        }

        public long BitRate { get; }

        [NotNull]
        public string ContentType { get; }

        public VideoContentType RecognizedContentType { get; }

        [NotNull]
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
