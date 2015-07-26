using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Cadena.Data
{
    /// <summary>
    /// Entity
    /// </summary>
    public class TwitterEntity
    {

        /// <summary>
        /// Parse entities json to parsed object enumeration.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="mediaUrlUseHttps">read media_url_https instead of media_url</param>
        /// <returns></returns>
        public static IEnumerable<TwitterEntity> ParseEntities(dynamic json,
            bool mediaUrlUseHttps = true)
        {
            if (json.IsDefined("hashtags"))
            {
                var tags = json.hashtags;
                foreach (var tag in tags)
                {
                    yield return new TwitterEntity(EntityType.Hashtags, tag.text,
                        null, null, null,
                        (int)tag.indices[0], (int)tag.indices[1]);
                }
            }
            if (json.IsDefined("media"))
            {
                var medias = json.media;
                foreach (var media in medias)
                {
                    var mediaUrl = mediaUrlUseHttps
                        ? media.media_url_https
                        : media.media_url;

                    yield return new TwitterEntity(EntityType.Hashtags, media.display_url,
                        media.url, mediaUrl, null,
                        (int)media.indices[0], (int)media.indices[1]);
                }
            }
            if (json.IsDefined("urls"))
            {
                var urls = json.urls;
                foreach (var url in urls)
                {
                    string display = url.url;
                    string expanded = url.url;
                    if (url.display_url())
                    {
                        display = url.display_url;
                    }
                    if (url.expanded_url())
                    {
                        expanded = url.expanded_url;
                    }
                    var orgurl = !String.IsNullOrEmpty(expanded) ? expanded : display;
                    yield return new TwitterEntity(EntityType.Hashtags,
                        orgurl, orgurl, null, null,
                        (int)url.indices[0], (int)url.indices[1]);
                }
            }
            if (json.IsDefined("user_mentions"))
            {
                var mentions = json.user_mentions;
                foreach (var mention in mentions)
                {
                    yield return new TwitterEntity(EntityType.UserMentions,
                        mention.screen_name, null, null, Int64.Parse(mention.id_str),
                        (int)mention.indices[0], (int)mention.indices[1]);
                }
            }
        }

        private TwitterEntity(EntityType type, [NotNull] string displayText,
            [CanBeNull] string originalUrl, [CanBeNull] string mediaUrl,
            long? userId, int startIndex, int endIndex)
        {
            if (displayText == null) throw new ArgumentNullException(nameof(displayText));
            EntityType = type;
            DisplayText = displayText;
            OriginalUrl = originalUrl;
            MediaUrl = mediaUrl;
            UserId = userId;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        /// <summary>
        /// Type of this entity.
        /// </summary>
        public EntityType EntityType { get; }

        /// <summary>
        /// Text for display to users.
        /// </summary>
        [NotNull]
        public string DisplayText { get; }

        /// <summary>
        /// Original(unshortened) URL (if this entity describes (shortened) url).
        /// </summary>
        [CanBeNull]
        public string OriginalUrl { get; }

        /// <summary>
        /// Numerical ID of the user (if this entity describes user).
        /// </summary>
        public long? UserId { get; }

        /// <summary>
        /// Url of media (if this entity describes media link).
        /// </summary>
        [CanBeNull]
        public string MediaUrl { get; }

        /// <summary>
        /// Start index of this entity
        /// </summary>
        public int StartIndex { get; }

        /// <summary>
        /// End index of this entity
        /// </summary>
        public int EndIndex { get; }
    }

    public enum EntityType
    {
        Media,
        Urls,
        UserMentions,
        Hashtags
    }
}
