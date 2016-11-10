using Cadena.Meteor;
using Cadena.Util;

namespace Cadena.Data
{
    public class TwitterUploadedMedia
    {
        public long MediaId { get; }

        public long? ExpireAfterSecs { get; }

        public long? Size { get; }

        public TwitterUploadedMediaPayload Payload { get; }

        public TwitterUploadedMedia(JsonValue json)
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
    }

    public abstract class TwitterUploadedMediaPayload { }

    public class TwitterUploadedPhotoPayload : TwitterUploadedMediaPayload
    {
        public int Width { get; }

        public int Height { get; }

        public string ImageType { get; }

        public TwitterUploadedPhotoPayload(JsonValue image)
        {
            Width = image["w"].AsInteger();
            Height = image["h"].AsInteger();
            ImageType = image["image_type"].AsString();
        }
    }

    public class TwitterUploadedVideoPayload : TwitterUploadedMediaPayload
    {
        public string VideoType { get; }

        public TwitterUploadedVideoPayload(JsonValue video)
        {
            VideoType = video["video_type"].AsString();
        }
    }
}
