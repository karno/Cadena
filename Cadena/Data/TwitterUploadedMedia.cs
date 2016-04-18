using Cadena.Meteor;
using Cadena.Util;

namespace Cadena.Data
{
    public class TwitterUploadedMedia
    {
        public long MediaId { get; }

        public int? ExpireAfterSecs { get; }

        public int? Size { get; }

        public TwitterUploadedMediaPayload Payload { get; }

        public TwitterUploadedMedia(JsonValue json)
        {
            MediaId = json["media_id_string"].AsString().ParseLong();
            var size = json["size"];
            Size = size.IsNumber ? (int)size.AsLong() : (int?)null;
            var expire = json["expires_after_secs"];
            ExpireAfterSecs = expire.IsNumber ? (int)expire.AsLong() : (int?)null;
            var image = json["image"].AsObject();
            var video = json["video"].AsObject();
            if (image != null)
            {
                Payload = new TwitterUploadedPhotoPayload(image);
            }
            else if (video != null)
            {
                Payload = new TwitterUploadedVideoPayload(video);
            }
        }
    }

    public abstract class TwitterUploadedMediaPayload { }

    public class TwitterUploadedPhotoPayload : TwitterUploadedMediaPayload
    {
        public int Width { get; }

        public int Height { get; }

        public string ImageType { get; }

        public TwitterUploadedPhotoPayload(dynamic image)
        {
            Width = image.w;
            Height = image.h;
            ImageType = image.image_type;
        }

        public TwitterUploadedPhotoPayload(JsonValue image)
        {
            Width = (int)image["w"].AsLong();
            Height = (int)image["h"].AsLong();
            ImageType = image["image_type"].AsString();
        }
    }

    public class TwitterUploadedVideoPayload : TwitterUploadedMediaPayload
    {
        public string VideoType { get; }

        public TwitterUploadedVideoPayload(dynamic video)
        {
            VideoType = video.video_type;
        }

        public TwitterUploadedVideoPayload(JsonValue video)
        {
            VideoType = video["video_type"].AsString();
        }
    }
}
