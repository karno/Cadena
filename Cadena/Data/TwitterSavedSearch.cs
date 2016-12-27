using System;
using Cadena.Meteor;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena.Data
{
    /// <summary>
    /// Saved search query
    /// </summary>
    public class TwitterSavedSearch
    {
        public TwitterSavedSearch(JsonValue json)
        {
            Id = json["id_str"].AsString().ParseLong();
            CreatedAt = json["created_at"].AsString()
                .ParseDateTime(ParsingExtension.TwitterDateTimeFormat);
            Query = json["query"].AsString().AssertNotNull("json.query could not be null.");
            Name = json["name"].AsString() ?? Query;
        }

        public TwitterSavedSearch(
            long id, DateTime createdAt, [NotNull] string query, [NotNull] string name)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (name == null) throw new ArgumentNullException(nameof(name));
            Id = id;
            CreatedAt = createdAt;
            Query = query;
            Name = name;
        }

        /// <summary>
        /// Saved user id
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Created at
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Saved search name
        /// </summary>
        [NotNull]
        public string Name { get; }

        /// <summary>
        /// Search query
        /// </summary>
        [NotNull]
        public string Query { get; }

        // "position" attribute is contained in object which twitter returns,
        // but that always indicates null.
    }
}
