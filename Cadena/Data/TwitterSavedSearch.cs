using System;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena.Data
{
    /// <summary>
    /// Saved search query
    /// </summary>
    public class TwitterSavedSearch
    {
        internal TwitterSavedSearch(dynamic json)
        {
            Id = ((string)json.id_str).ParseLong();
            CreatedAt = ((string)json.created_at)
                .ParseDateTime(ParsingExtension.TwitterDateTimeFormat);
            Name = json.name;
            Query = json.query;
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
