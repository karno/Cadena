using System;
using System.Collections.Generic;
using System.Linq;
using Cadena.Data;

namespace Cadena.Util
{
    public static class TextEntityResolver
    {
        public static IEnumerable<TextEntityDescription> ParseText(TwitterStatus status)
        {
            if (status == null) throw new ArgumentNullException(nameof(status));
            var entities = status.Entities ?? new TwitterEntity[0];
            return ParseText(status.Text, entities);
        }

        public static IEnumerable<TextEntityDescription> ParseText(
            string text, IEnumerable<TwitterEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            var escaped = ParsingExtension.EscapeEntity(text);
            var endIndex = 0;

            // distinct by startindex ignores extended_entities.
            foreach (var entity in entities.Distinct(e => e.StartIndex).OrderBy(e => e.StartIndex))
            {
                if (endIndex < entity.StartIndex)
                {
                    // return raw string
                    yield return new TextEntityDescription(ParsingExtension.ResolveEntity(
                        escaped.SurrogatedSubstring(endIndex, entity.StartIndex - endIndex)));
                }
                // get entitied text
                var body = ParsingExtension.ResolveEntity(escaped.SurrogatedSubstring(
                    entity.StartIndex, entity.EndIndex - entity.StartIndex));
                yield return new TextEntityDescription(body, entity);
                endIndex = entity.EndIndex;
            }
            if (endIndex == 0)
            {
                // entity is empty.
                yield return new TextEntityDescription(text);
            }
            else if (endIndex < escaped.Length)
            {
                // return remain text
                yield return new TextEntityDescription(ParsingExtension.ResolveEntity(
                    escaped.SurrogatedSubstring(endIndex)));
            }
        }

        // below code from Mystique pull request #53.
        // Thanks for Hotspring-r
        // https://github.com/karno/Mystique/commit/a8d174bcfe9292290bd9058ecf7ce2b68dc4162e
    }

    public class TextEntityDescription
    {
        public TextEntityDescription(string text, TwitterEntity entity = null)
        {
            Text = text;
            Entity = entity;
        }

        public string Text { get; }

        public TwitterEntity Entity { get; }

        public bool IsEntityAvailable
        {
            get { return this.Entity != null; }
        }
    }
}
