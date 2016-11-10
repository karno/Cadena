using System;
using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena.Data.Entities
{
    public sealed class UrlEntity : TwitterEntity
    {
        public UrlEntity(JsonValue json) : base(json)
        {
            Url = json["url"].AsStringOrNull();
            DisplayUrl = json["display_url"].AsStringOrNull();
            ExpandedUrl = json["expanded_url"].AsStringOrNull();
            DisplayText = DisplayUrl ?? String.Empty;
            FullText = ExpandedUrl ?? DisplayText;
        }

        public override string DisplayText { get; }

        public override string FullText { get; }

        /// <summary>
        /// Represents t.co url
        /// </summary>
        [CanBeNull]
        public string Url { get; }

        /// <summary>
        /// Represents shortened url
        /// </summary>
        [CanBeNull]
        public string DisplayUrl { get; }

        /// <summary>
        /// Represents original (fully-expanded) url
        /// </summary>
        [CanBeNull]
        public string ExpandedUrl { get; }
    }
}
