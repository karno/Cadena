using System;
using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena.Data
{
    public class TwitterConfiguration
    {
        internal TwitterConfiguration(dynamic json)
        {
            CharactersReservedPerMedia = (int)json.characters_reserved_per_media;
            PhotoSizeLimit = (int)json.photo_size_limit;
            NonUserPaths = (string[])json.non_user_paths;
            ShortUrlLength = (int)json.short_url_length;
            ShortUrlLengthHttps = (int)json.short_url_length_https;
            // MaxMediaPerUpload = json.max_media_per_upload;
            if (NonUserPaths == null)
            {
                throw new ArgumentException("json.non_user_paths could not be null.");
            }
        }

        internal TwitterConfiguration(JsonValue json)
        {
            CharactersReservedPerMedia = (int)json["characters_reserved_per_media"].AsLong();
            PhotoSizeLimit = (int)json["photo_size_limit"].AsLong();
            NonUserPaths = json["non_user_paths"].AsArray()?.AsStringArray() ?? new string[0];
            ShortUrlLength = (int)json["short_url_length"].AsLong();
            ShortUrlLengthHttps = (int)json["short_url_length_https"].AsLong();
            // MaxMediaPerUpload = (int)json["max_media_per_upload"].AsLong();
            if (NonUserPaths == null)
            {
                throw new ArgumentException("json.non_user_paths could not be null.");
            }
        }

        /// <summary>
        /// URL length for uploaded media
        /// </summary>
        public int CharactersReservedPerMedia { get; }

        /// <summary>
        /// Reserved user screen_names (reserved by twitter)
        /// </summary>
        [NotNull]
        public string[] NonUserPaths { get; }

        /// <summary>
        /// Photo size  limit (bytes)
        /// </summary>
        public int PhotoSizeLimit { get; }

        /// <summary>
        /// Shortened url length
        /// </summary>
        public int ShortUrlLength { get; }

        /// <summary>
        /// Shortened url length(https version, generally ShortUrlLength + 1)
        /// </summary>
        public int ShortUrlLengthHttps { get; }

        /// <summary>
        /// max media count
        /// </summary>
        [Obsolete("This property is no longer updated.")]
        public int MaxMediaPerUpload { get; }
    }
}
