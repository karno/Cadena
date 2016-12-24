using Cadena.Meteor;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena.Data
{
    public class TwitterUploadedMedia
    {
        public long MediaId { get; }

        public long? ExpireAfterSecs { get; }

        public long? Size { get; }

        public TwitterUploadedMediaPayload Payload { get; }

        internal TwitterUploadedMedia(JsonValue json)
        {
            MediaId = json["media_id_string"].AsString().ParseLong();
            var size = json["size"];
            Size = size.AsLongOrNull();
            ExpireAfterSecs = json["expires_after_secs"].AsLongOrNull();
            var image = json["image"].AsObjectOrNull();
            var video = json["video"].AsObjectOrNull();
            if (image != null)
            {
                Payload = new TwitterUploadedPhotoPayload(image);
            }
            else if (video != null)
            {
                Payload = new TwitterUploadedVideoPayload(video);
            }
        }

        public TwitterUploadedMedia(
            long mediaId, long? expireAfterSecs, long? size,
            TwitterUploadedMediaPayload payload)
        {
            MediaId = mediaId;
            ExpireAfterSecs = expireAfterSecs;
            Size = size;
            Payload = payload;
        }
    }

    public abstract class TwitterUploadedMediaPayload
    {
    }

    public class TwitterUploadedPhotoPayload : TwitterUploadedMediaPayload
    {
        public int Width { get; }

        public int Height { get; }

        [CanBeNull]
        public string ImageType { get; }

        internal TwitterUploadedPhotoPayload(JsonValue image)
        {
            Width = image["w"].AsInteger();
            Height = image["h"].AsInteger();
            ImageType = image["image_type"].AsStringOrNull();
        }
        public TwitterUploadedPhotoPayload(int width, int height, [CanBeNull] string imageType)
        {
            Width = width;
            Height = height;
            ImageType = imageType;
        }
    }

    public class TwitterUploadedVideoPayload : TwitterUploadedMediaPayload
    {
        [CanBeNull]
        public string VideoType { get; }

        internal TwitterUploadedVideoPayload(JsonValue video)
        {
            VideoType = video["video_type"].AsString();
        }

        public TwitterUploadedVideoPayload([CanBeNull] string videoType)
        {
            VideoType = videoType;
        }
    }
}
