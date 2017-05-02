using System;
using System.Linq;
using Cadena.Data.Entities;
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

        public TwitterStatus(JsonValue json)
        {
            // read numeric id and timestamp
            Id = json["id_str"].AsString().ParseLong();
            CreatedAt = json["created_at"].AsString().ParseDateTime(ParsingExtension.TwitterDateTimeFormat);

            // check extended_tweet is existed
            var exjson = json.ContainsKey("extended_tweet") ? json["extended_tweet"] : json;

            // read full_text ?? text
            var text = exjson.ContainsKey("full_text") ? exjson["full_text"] : exjson["text"];
            Text = ParsingExtension.ResolveEntity(text.AsString());

            var array = exjson["display_text_range"].AsArrayOrNull()?.AsLongArray();
            if (array != null && array.Length >= 2)
            {
                DisplayTextRange = new Tuple<int, int>((int)array[0], (int)array[1]);
            }

            if (exjson.ContainsKey("extended_entities"))
            {
                // get correctly typed entities array
                var orgEntities = TwitterEntity.ParseEntities(json["entities"]).ToArray();
                var extEntities = TwitterEntity.ParseEntities(json["extended_entities"]).ToArray();

                // merge entities
                Entities = orgEntities.Where(e => !(e is TwitterMediaEntity))
                                      .Concat(extEntities) // extended entities contains media entities only.
                                      .ToArray();
            }
            else if (exjson.ContainsKey("entities"))
            {
                Entities = TwitterEntity.ParseEntities(exjson["entities"]).ToArray();
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
                FavoriteCount = json["favorite_count"].AsLongOrNull();
                RetweetCount = json["retweet_count"].AsLongOrNull();
                InReplyToScreenName = json["in_reply_to_screen_name"].AsString();

                if (json.ContainsKey("retweeted_status"))
                {
                    var retweeted = new TwitterStatus(json["retweeted_status"]);
                    RetweetedStatus = retweeted;
                    // merge text and entities
                    Text = retweeted.Text;
                    Entities = retweeted.Entities;
                }
                if (json.ContainsKey("quoted_status"))
                {
                    var quoted = new TwitterStatus(json["quoted_status"]);
                    QuotedStatus = quoted;
                }
                var coordinates = json["coordinates"]["coordinates"].AsArrayOrNull()?.AsDoubleArray();
                if (coordinates != null && coordinates.Length >= 2)
                {
                    Coordinates = Tuple.Create(coordinates[0], coordinates[1]);
                }
            }
        }

        public TwitterStatus(
            long id, [NotNull] TwitterUser user, [NotNull] string text,
            [CanBeNull] Tuple<int, int> displayTextRange, DateTime createdAt, [NotNull] TwitterEntity[] entities,
            [CanBeNull] string source, long? inReplyToStatusId, long? inReplyToUserId,
            long? favoritedCount, long? retweetedCount,
            [CanBeNull] string inReplyToScreenName, [CanBeNull] Tuple<double, double> coordinates,
            [CanBeNull] TwitterStatus retweetedStatus, [CanBeNull] TwitterStatus quotedStatus)
            : this(id, StatusType.Tweet, user, text, displayTextRange, createdAt, entities)
        {
            Source = source;
            InReplyToStatusId = inReplyToStatusId;
            InReplyToScreenName = inReplyToScreenName;
            InReplyToUserId = inReplyToUserId;
            FavoriteCount = favoritedCount;
            RetweetCount = retweetedCount;
            Coordinates = coordinates;
            RetweetedStatus = retweetedStatus;
            QuotedStatus = quotedStatus;
        }

        public TwitterStatus(
            long id, [NotNull] TwitterUser user, [NotNull] TwitterUser recipient,
            [NotNull] string text, [CanBeNull] Tuple<int, int> displayTextRange, DateTime createdAt,
            [NotNull] TwitterEntity[] entities)
            : this(id, StatusType.DirectMessage, user, text, displayTextRange, createdAt, entities)
        {
            Recipient = recipient;
        }

        private TwitterStatus(
            long id, StatusType statusType, [NotNull] TwitterUser user, [NotNull] string text,
            [CanBeNull] Tuple<int, int> displayTextRange, DateTime createdAt, [NotNull] TwitterEntity[] entities)
        {
            Id = id;
            StatusType = statusType;
            User = user ?? throw new ArgumentNullException(nameof(user));
            User = user;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            DisplayTextRange = displayTextRange;
            CreatedAt = createdAt;
            Entities = entities ?? throw new ArgumentNullException(nameof(entities));
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
        public Tuple<int, int> DisplayTextRange { get; }

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
        /// Count of how many times this status has favorited.
        /// </summary>
        public long? FavoriteCount { get; }

        /// <summary>
        /// Count of how many times this status has retweeted.
        /// </summary>
        public long? RetweetCount { get; }

        /// <summary>
        /// Geographic point that is associated with this status, [Latitude, Longitude].
        /// </summary>
        [CanBeNull]
        public Tuple<double, double> Coordinates { get; }

        /// <summary>
        /// The status that is retweeted by this status
        /// </summary>
        [CanBeNull]
        public TwitterStatus RetweetedStatus { get; }

        /// <summary>
        /// The status that is quoted by status
        /// </summary>
        [CanBeNull]
        public TwitterStatus QuotedStatus { get; }

        #endregion Properties for statuses

        #region Properties for direct messages

        /// <summary>
        /// Recipient of message. (ONLY FOR DIRECT MESSAGE)
        /// </summary>
        [CanBeNull]
        public TwitterUser Recipient { get; }

        #endregion Properties for direct messages

        /// <summary>
        /// Entity objects of the status
        /// </summary>
        [NotNull]
        public TwitterEntity[] Entities { get; }

        /// <summary>
        /// Web URL for accessing status
        /// </summary>
        [NotNull]
        public string Permalink => String.Format(TwitterStatusUrl, User.ScreenName, Id);

        // ReSharper disable InconsistentNaming
        [NotNull]
        public string STOTString => "@" + User.ScreenName + ": " + Text + " [" + Permalink + "]";

        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Get entity-applied text
        /// </summary>
        /// <param name="displayMode">switch of replacer text</param>
        /// <returns></returns>
        [NotNull]
        public string GetEntityAidedText(EntityDisplayMode displayMode = EntityDisplayMode.DisplayText)
        {
            var status = RetweetedStatus ?? this;
            return TextEntityResolver.GetEntityAidedText(status.Text, status.Entities, displayMode);
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
    }
}