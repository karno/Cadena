using System;
using Cadena.Meteor;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena.Data
{
    public class TwitterList
    {
        private const string TwitterListUriPrefix = "http://twitter.com";

        internal TwitterList(JsonValue json)
        {
            Id = json["id_str"].AsString().ParseLong();
            User = new TwitterUser(json["user"]);
            Name = json["name"].AsString() ?? String.Empty;
            FullName = json["full_name"].AsString() ?? String.Empty;
            Uri = new Uri(TwitterListUriPrefix + json["uri"].AsString());
            Slug = json["slug"].AsString().AssertNotNull("json.slug could not be null.");
            ListMode = json["mode"].AsString() == "public"
                ? ListMode.Public
                : ListMode.Private;
            Description = json["description"].AsString() ?? String.Empty;
            MemberCount = json["member_count"].AsLong();
            SubscriberCount = json["subscriber_count"].AsLong();
            CreatedAt = json["created_at"].AsString().ParseDateTime(ParsingExtension.TwitterDateTimeFormat);
        }

        /// <summary>
        /// ID of this list.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Created user
        /// </summary>
        [NotNull]
        public TwitterUser User { get; }

        /// <summary>
        /// Name of this list.
        /// </summary>
        [NotNull]
        public string Name { get; }

        /// <summary>
        /// Full name of this list.
        /// </summary>
        [NotNull]
        public string FullName { get; }

        /// <summary>
        /// Uri for this list.
        /// </summary>
        [NotNull]
        public Uri Uri { get; }

        /// <summary>
        /// Slug of this list.
        /// </summary>
        [NotNull]
        public string Slug { get; }

        /// <summary>
        /// State of this list.
        /// </summary>
        public ListMode ListMode { get; }

        /// <summary>
        /// Description of this list.
        /// </summary>
        [NotNull]
        public string Description { get; }

        /// <summary>
        /// Sum of members in this list.
        /// </summary>
        public long MemberCount { get; }

        /// <summary>
        /// Sum of subscribers this list.
        /// </summary>
        public long SubscriberCount { get; }

        /// <summary>
        /// Created timestamp of this list.
        /// </summary>
        public DateTime CreatedAt { get; }
    }

    public enum ListMode
    {
        Public,
        Private
    }
}