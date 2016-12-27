using System;
using System.Collections.Generic;
using System.Linq;
using Cadena.Data.Entities;
using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena.Data
{
    public abstract class TwitterEntity
    {
        public static IEnumerable<TwitterEntity> ParseEntities([CanBeNull] JsonValue json)
        {
            if (json == null) return Enumerable.Empty<TwitterEntity>();
            var tags = ParseSubEntities(json, "hashtags", t => new HashtagEntity(t));
            var symbols = ParseSubEntities(json, "symbols", s => new SymbolEntity(s));
            var urls = ParseSubEntities(json, "urls", u => new UrlEntity(u));
            var mentions = ParseSubEntities(json, "user_mentions", m => new UserMentionEntity(m));
            var media = ParseSubEntities(json, "media", m => new MediaEntity(m));
            return new[] { tags, symbols, urls, mentions, media }.SelectMany(e => e);
        }

        [NotNull]
        private static IEnumerable<TwitterEntity> ParseSubEntities(
            [NotNull] JsonValue node, [NotNull] string key,
            [NotNull] Func<JsonValue, TwitterEntity> entityFactory)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (entityFactory == null) throw new ArgumentNullException(nameof(entityFactory));
            return node[key].AsArrayOrNull()?.Select(entityFactory) ?? Enumerable.Empty<TwitterEntity>();
        }

        protected TwitterEntity(JsonValue json)
        {
            var indices = json["indices"].AsArrayOrNull()?.AsLongArray();
            if (indices == null || indices.Length < 2)
            {
                throw new ArgumentException("this entity object not contains indices element or too short.");
            }
            Indices = Tuple.Create((int)indices[0], (int)indices[1]);
        }

        protected TwitterEntity(Tuple<int, int> indices)
        {
            Indices = indices;
        }

        /// <summary>
        /// Represents (StartIndex, EndIndex).
        /// </summary>
        [NotNull]
        public Tuple<int, int> Indices { get; }

        [NotNull]
        public virtual string DisplayText => FullText;

        [NotNull]
        public abstract string FullText { get; }

        public override string ToString()
        {
            return $"{Indices} {DisplayText}";
        }
    }
}
