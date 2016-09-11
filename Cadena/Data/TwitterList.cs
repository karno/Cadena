using System;
using Cadena.Meteor;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena.Data
{
    public class TwitterList
    {
        private const string TwitterListUriPrefix = "http://twitter.com";

        internal TwitterList(dynamic json)
        {
            Id = Int64.Parse(json.id_str);
            User = new TwitterUser(json.user);
            Name = json.name;
            FullName = json.full_name;
            Uri = new Uri(TwitterListUriPrefix + json.uri);
            Slug = json.slug;
            ListMode = String.Equals(json.mode, "public", StringComparison.OrdinalIgnoreCase)
                           ? ListMode.Public
                           : ListMode.Private;
            Description = json.description;
            MemberCount = (long)json.member_count;
            SubscriberCount = (long)json.subscriber_count;
            CreatedAt = ((string)json.created_at).ParseDateTime(ParsingExtension.TwitterDateTimeFormat);
            // check null 
            if (User == null)
            {
                throw new ArgumentException("json.user could not be null.");
            }
            if (Name == null)
            {
                throw new ArgumentException("json.name could not be null.");
            }
            if (FullName == null)
            {
                throw new ArgumentException("json.full_name could not be null.");
            }
            if (Uri == null)
            {
                throw new ArgumentException("json.uri could not be null.");
            }
            if (Slug == null)
            {
                throw new ArgumentException("json.slug could not be null.");
            }
            if (Description == null)
            {
                throw new ArgumentException("json.description could not be null.");
            }
        }

        internal TwitterList(JsonValue json)
        {
            Id = json["id_str"].AsString().ParseLong();
            User = new TwitterUser(json["user"]);
            Name = json["name"].AsString().AssertNotNull("json.name could not be null.");
            FullName = json["full_name"].AsString().AssertNotNull("json.full_name could not be null.");
            Uri = new Uri(TwitterListUriPrefix + json["uri"].AsString().AssertNotNull("json.url could not be null."));
            Slug = json["slug"].AsString().AssertNotNull("json.slug could not be null.");
            ListMode = json["mode"].AsString() == "public"
                ? ListMode.Public
                : ListMode.Private;
            Description = json["description"].AsString().AssertNotNull("json.description could not be null.");
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