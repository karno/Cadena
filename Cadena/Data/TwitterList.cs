using System;
using Cadena.Meteor;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena.Data
{
    public class TwitterList
    {
        private const string TwitterListUriPrefix = "http://twitter.com";

        public TwitterList(JsonValue json)
        {
            Id = json["id_str"].AsString().ParseLong();
            User = new TwitterUser(json["user"]);
            Name = json["name"].AsString();
            FullName = json["full_name"].AsString();
            Uri = new Uri(TwitterListUriPrefix + json["uri"].AsString());
            Slug = json["slug"].AsString();
            ListMode = json["mode"].AsString() == "public"
                ? ListMode.Public
                : ListMode.Private;
            Description = json["description"].AsStringOrNull() ?? String.Empty;
            MemberCount = json["member_count"].AsLong();
            SubscriberCount = json["subscriber_count"].AsLong();
            CreatedAt = json["created_at"].AsString().ParseDateTime(ParsingExtension.TwitterDateTimeFormat);
        }

        public TwitterList(
            long id, [NotNull] TwitterUser user, [NotNull] string name, [NotNull] string fullName,
            [NotNull] Uri uri, [NotNull] string slug, ListMode mode, [CanBeNull] string description,
            long memberCount, long subscriberCount, DateTime createdAt)
        {
            Id = id;
            User = user ?? throw new ArgumentNullException(nameof(user));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            Slug = slug ?? throw new ArgumentNullException(nameof(slug));
            ListMode = mode;
            Description = description ?? String.Empty;
            MemberCount = memberCount;
            SubscriberCount = subscriberCount;
            CreatedAt = createdAt;
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