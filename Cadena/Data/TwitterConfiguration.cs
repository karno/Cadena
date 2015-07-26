using System;
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
#pragma warning disable 618
            MaxMediaPerUpload = json.max_media_per_upload;
#pragma warning restore 618
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
        [Obsolete("This property is no longer update?")]
        public int MaxMediaPerUpload { get; }
    }
}
