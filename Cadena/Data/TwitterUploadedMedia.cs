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

        public TwitterUploadedMedia(dynamic json)
        {
            MediaId = ((string)json.media_id_string).ParseLong();
            Size = json.size() ? (int)json.size : (int?)null;
            ExpireAfterSecs = json.expires_after_secs() ? (int)json.expires_after_secs : (int?)null;
            if (json.image())
            {
                Payload = new TwitterUploadedPhotoPayload(json.image);
            }
            else if (json.video())
            {
                Payload = new TwitterUploadedVideoPayload(json.video);
            }
        }

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
