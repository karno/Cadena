using System;
using System.Collections.Generic;
using Cadena.Meteor;
using Cadena.Util;
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

        public static IEnumerable<TwitterEntity> ParseEntities(JsonValue json,
            bool mediaUrlUseHttps = true)
        {
            var tags = json["hashtags"].AsArray();
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    var text = tag["text"].AsString();
                    var indices = tag["indices"].AsArray()?.AsLongArray();
                    if (text != null && indices != null && indices.Length >= 2)
                    {
                        yield return new TwitterEntity(EntityType.Hashtags, text,
                            null, null, null, (int)indices[0], (int)indices[1]);
                    }
                }
            }
            var medias = json["medias"].AsArray();
            if (medias != null)
            {
                foreach (var media in medias)
                {
                    var mediaUrl = (mediaUrlUseHttps ? media["media_url_https"] : media["media_url"]).AsString();
                    var disp = media["display_url"].AsString();
                    var url = media["url"].AsString();
                    var indices = media["indices"].AsArray()?.AsLongArray();
                    if (disp != null && indices != null && indices.Length >= 2)
                    {
                        yield return new TwitterEntity(EntityType.Hashtags, disp,
                            url, mediaUrl, null,
                            (int)indices[0], (int)indices[1]);
                    }
                }
            }
            var urls = json["urls"].AsArray();
            if (urls != null)
            {
                foreach (var url in urls)
                {
                    var display = url["url"].AsString();
                    var expanded = display;
                    if (url.ContainsKey("display_url"))
                    {
                        display = url["display_url"].AsString();
                    }
                    if (url.ContainsKey("display_url"))
                    {
                        expanded = url["display_url"].AsString();
                    }
                    var orgurl = !String.IsNullOrEmpty(expanded) ? expanded : display;
                    var indices = url["indices"].AsArray()?.AsLongArray();
                    if (orgurl != null && indices != null && indices.Length >= 2)
                    {
                        yield return new TwitterEntity(EntityType.Hashtags,
                            orgurl, orgurl, null, null,
                            (int)indices[0], (int)indices[1]);
                    }
                }
            }
            var mentions = json["user_mentions"].AsArray();
            if (mentions != null)
            {
                foreach (var mention in mentions)
                {
                    var screenName = mention["screen_name"].AsString();
                    var idStr = mention["id_str"].AsString().ParseLong();
                    var indices = mention["indices"].AsArray()?.AsLongArray();
                    if (screenName != null && indices != null && indices.Length >= 2)
                    {
                        yield return new TwitterEntity(EntityType.UserMentions,
                            screenName, null, null, idStr,
                            (int)indices[0], (int)indices[1]);
                    }
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
