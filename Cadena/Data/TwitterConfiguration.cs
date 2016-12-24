using System;
using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena.Data
{
    public class TwitterConfiguration
    {
        internal TwitterConfiguration(JsonValue json)
        {
            CharactersReservedPerMedia = (int)json["characters_reserved_per_media"].AsLong();
            NonUserPaths = json["non_user_paths"].AsArrayOrNull()?.AsStringArray() ?? new string[0];
            PhotoSizeLimit = (int)json["photo_size_limit"].AsLong();
            ShortUrlLength = (int)json["short_url_length"].AsLong();
            ShortUrlLengthHttps = (int)json["short_url_length_https"].AsLong();
            // MaxMediaPerUpload = (int)json["max_media_per_upload"].AsLong();
            if (NonUserPaths == null)
            {
                throw new ArgumentException("json.non_user_paths could not be null.");
            }
        }

        public TwitterConfiguration(
            int charactersReservedPerMedia, [NotNull] string[] nonUserPaths,
            int photoSizeLimit, int shortUrlLength, int shortUrlLengthHttps)
        {
            if (nonUserPaths == null) throw new ArgumentNullException(nameof(nonUserPaths));
            CharactersReservedPerMedia = charactersReservedPerMedia;
            NonUserPaths = nonUserPaths;
            PhotoSizeLimit = photoSizeLimit;
            ShortUrlLength = shortUrlLength;
            ShortUrlLengthHttps = shortUrlLengthHttps;
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
