using System;
using System.Linq;
using System.Text;
using Cadena.Meteor;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena.Data
{
    /// <summary>
    /// Represents twitter status.
    /// </summary>
    public class TwitterStatus
    {
        public const string TwitterStatusUrl = "https://twitter.com/{0}/status/{1}";

        internal TwitterStatus(JsonValue json)
        {
            // read numeric id and timestamp
            Id = json["id_str"].AsString().ParseLong();
            CreatedAt = json["created_at"].AsString().ParseDateTime(ParsingExtension.TwitterDateTimeFormat);

            // check extended_tweet is existed
            var exjson = json.ContainsKey("extended_tweet") ? json["extended_tweet"] : json;

            // read full_text ?? text
            var text = exjson.ContainsKey("full_text") ? exjson["full_text"] : exjson["text"];
            Text = ParsingExtension.ResolveEntity(text.AsString());

            var array = exjson["display_text_range"].AsArray()?.AsLongArray();
            if (array != null && array.Length >= 2)
            {
                DisplayTextRange = new Tuple<long, long>(array[0], array[1]);
            }

            if (exjson.ContainsKey("extended_entities"))
            {
                // get correctly typed entities array
                var orgEntities = TwitterEntity.ParseEntities(json["entities"]).ToArray();
                var extEntities = TwitterEntity.ParseEntities(json["extended_entities"]).ToArray();

                // merge entities
                Entities = orgEntities.Where(e => e.EntityType != EntityType.Media)
                                           .Concat(extEntities) // extended entities contains media entities only.
                                           .ToArray();
            }
            else if (exjson.ContainsKey("entities"))
            {
                Entities = TwitterEntity.ParseEntities(json["entities"]).ToArray();
            }
            else
            {
                Entities = new TwitterEntity[0];
            }
            if (json.ContainsKey("recipient"))
            {
                // THIS IS DIRECT MESSAGE!
                StatusType = StatusType.DirectMessage;
                User = new TwitterUser(json["sender"]);
                Recipient = new TwitterUser(json["recipient"]);
            }
            else
            {
                StatusType = StatusType.Tweet;
                User = new TwitterUser(json["user"]);
                Source = json["source"].AsString();
                InReplyToStatusId = json["in_reply_to_status_id_str"].AsString().ParseNullableId();
                InReplyToUserId = json["in_reply_to_user_id_str"].AsString().ParseNullableId();
                InReplyToScreenName = json["in_reply_to_screen_name"].AsString();

                if (json.ContainsKey("retweeted_status"))
                {
                    var retweeted = new TwitterStatus(json["retweeted_status"]);
                    RetweetedStatus = retweeted;
                    RetweetedStatusId = retweeted.Id;
                    // merge text and entities
                    Text = retweeted.Text;
                    Entities = retweeted.Entities;
                }
                if (json.ContainsKey("quoted_status"))
                {
                    var quoted = new TwitterStatus(json["quoted_status"]);
                    QuotedStatus = quoted;
                    QuotedStatusId = quoted.Id;
                }
                var coordinates = json["coordinates"].AsArray()?.AsDoubleArray();
                if (coordinates != null)
                {
                    Longitude = coordinates[0];
                    Latitude = coordinates[1];
                }
            }
        }

        /// <summary>
        /// Sequential ID of tweet/message.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// The flag indicated whether status is a tweet or a message.
        /// </summary>
        public StatusType StatusType { get; }

        /// <summary>
        /// Author of tweet/message.
        /// </summary>
        [NotNull]
        public TwitterUser User { get; }

        /// <summary>
        /// Body of tweet/message. Escape characters are already resolved.
        /// </summary>
        [NotNull]
        public string Text { get; }

        /// <summary>
        /// Display text range is specified, or null.
        /// </summary>
        [CanBeNull]
        public Tuple<long, long> DisplayTextRange { get; }

        /// <summary>
        /// Created timestamp of the status.
        /// </summary>
        public DateTime CreatedAt { get; }

        #region Properties for statuses

        /// <summary>
        /// Source of the status. (a.k.a. via, from, ...)
        /// </summary>
        [CanBeNull]
        public string Source { get; }

        /// <summary>
        /// Status ID that is replied from this status.
        /// </summary>
        public long? InReplyToStatusId { get; }

        /// <summary>
        /// User ID that is replied from this status.
        /// </summary>
        public long? InReplyToUserId { get; }

        /// <summary>
        /// User screen name that is replied from this status.
        /// </summary>
        [CanBeNull]
        public string InReplyToScreenName { get; }

        /// <summary>
        /// Latitude of geographic point that is associated with this status.
        /// </summary>
        public double? Latitude { get; }

        /// <summary>
        /// Longitude of geographic point that is associated with this status.
        /// </summary>
        public double? Longitude { get; }

        /// <summary>
        /// The id of the status that is retweeted by this status
        /// </summary>
        public long? RetweetedStatusId { get; }

        /// <summary>
        /// The status that is retweeted by this status
        /// </summary>
        [CanBeNull]
        public TwitterStatus RetweetedStatus { get; }

        /// <summary>
        /// The id of the status that is quoted by this status
        /// </summary>
        public long? QuotedStatusId { get; }

        /// <summary>
        /// The status that is quoted by status
        /// </summary>
        [CanBeNull]
        public TwitterStatus QuotedStatus { get; }
        #endregion

        #region Properties for direct messages

        /// <summary>
        /// Recipient of message. (ONLY FOR DIRECT MESSAGE)
        /// </summary>
        [CanBeNull]
        public TwitterUser Recipient { get; }

        #endregion

        /// <summary>
        /// Entity objects of the status
        /// </summary>
        [CanBeNull]
        public TwitterEntity[] Entities { get; }

        /// <summary>
        /// Web URL for accessing status
        /// </summary>
        [NotNull]
        public string Permalink
        {
            get { return String.Format(TwitterStatusUrl, User.ScreenName, Id); }
        }

        // ReSharper disable InconsistentNaming
        [NotNull]
        public string STOTString
        {
            get
            {
                return "@" + User.ScreenName + ": " + Text + " [" + Permalink + "]";
            }
        }
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Get entity-applied text
        /// </summary>
        /// <param name="displayMode">switch of replacer text</param>
        /// <returns></returns>
        [NotNull]
        public string GetEntityAidedText(EntityDisplayMode displayMode = EntityDisplayMode.DisplayText)
        {
            try
            {
                var builder = new StringBuilder();
                var status = this;
                if (status.RetweetedStatus != null)
                {
                    // change target
                    status = status.RetweetedStatus;
                }
                foreach (var description in TextEntityResolver.ParseText(status))
                {
                    if (!description.IsEntityAvailable)
                    {
                        builder.Append(description.Text);
                    }
                    else
                    {
                        var entity = description.Entity;
                        switch (entity.EntityType)
                        {
                            case EntityType.Hashtags:
                                builder.Append("#" + entity.DisplayText);
                                break;
                            case EntityType.Urls:
                                // url entity:
                                // display_url: example.com/CUTTED OFF...
                                // original_url => expanded_url: example.com/full_original_url
                                builder.Append(displayMode != EntityDisplayMode.DisplayText
                                               && entity.OriginalUrl != null
                                    ? ParsingExtension.ResolveEntity(entity.OriginalUrl)
                                    : ParsingExtension.ResolveEntity(entity.DisplayText));
                                break;
                            case EntityType.Media:
                                // media entity:
                                // display_url: pic.twitter.com/IMAGE_ID
                                // media_url: pbs.twimg.com/media/ACTUAL_IMAGE_RESOURCE_ID
                                // url: t.co/IMAGE_ID
                                builder.Append(
                                    displayMode == EntityDisplayMode.LinkUri &&
                                    entity.MediaUrl != null
                                        ? ParsingExtension.ResolveEntity(entity.MediaUrl)
                                        : ParsingExtension.ResolveEntity(entity.DisplayText));
                                break;
                            case EntityType.UserMentions:
                                builder.Append("@" + entity.DisplayText);
                                break;
                        }
                    }
                }
                return builder.ToString();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Parse Error! : " + Text);
                if (Entities == null)
                {
                    sb.AppendLine("Entities: null");
                }
                else
                {
                    sb.Append("Entities: ");
                    foreach (var e in Entities.OrderBy(e => e.StartIndex))
                    {
                        sb.AppendLine("    " + e.StartIndex + "- " + e.EndIndex + " : " + e.DisplayText);
                    }
                }
                throw new ArgumentOutOfRangeException(sb.ToString(), ex);
            }
        }

        /// <summary>
        /// Get formatted tweet: &quot;@user: text&quot;
        /// </summary>
        public override string ToString()
        {
            return "@" + User.ScreenName + ": " + GetEntityAidedText();
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Id == ((TwitterStatus)obj).Id;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    /// <summary>
    /// Type of status
    /// </summary>
    public enum StatusType
    {
        /// <summary>
        /// Status is normal tweet.
        /// </summary>
        Tweet,
        /// <summary>
        /// Status is direct message.
        /// </summary>
        DirectMessage,
    }

    public enum EntityDisplayMode
    {
        DisplayText,
        LinkUri,
        MediaUri
    }
}