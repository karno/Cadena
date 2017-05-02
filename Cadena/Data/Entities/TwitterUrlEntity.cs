using System;
using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena.Data.Entities
{
    public sealed class TwitterUrlEntity : TwitterEntity
    {
        public TwitterUrlEntity(JsonValue json) : base(json)
        {
            Url = json["url"].AsStringOrNull();
            DisplayUrl = json["display_url"].AsStringOrNull();
            ExpandedUrl = json["expanded_url"].AsStringOrNull();
        }

        public TwitterUrlEntity(Tuple<int, int> indices,
            [CanBeNull] string url, [CanBeNull] string displayUrl, [CanBeNull] string expandUrl)
            : base(indices)
        {
            Url = url;
            DisplayUrl = displayUrl;
            ExpandedUrl = expandUrl;
        }

        public override string DisplayText => DisplayUrl ?? String.Empty;

        public override string FullText => ExpandedUrl ?? DisplayText;

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