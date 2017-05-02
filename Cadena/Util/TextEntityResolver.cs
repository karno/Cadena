using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cadena.Data;
using Cadena.Data.Entities;
using JetBrains.Annotations;

namespace Cadena.Util
{
    public static class TextEntityResolver
    {
        public static string GetEntityAidedText(string text, IEnumerable<TwitterEntity> entities,
            EntityDisplayMode displayMode)
        {
            try
            {
                var builder = new StringBuilder();
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var description in ParseText(text, entities))
                {
                    if (description.Entity != null)
                    {
                        builder.Append(displayMode == EntityDisplayMode.DisplayText
                            ? description.Entity.DisplayText
                            : description.Entity.FullText);
                    }
                    else
                    {
                        builder.Append(description.Text);
                    }
                }
                return builder.ToString();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Parse Error! : " + text);
                sb.Append("Entities: ");
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var e in entities.OrderBy(e => e.Indices.Item1))
                {
                    sb.AppendLine("    " + e);
                }
                throw new ArgumentOutOfRangeException(sb.ToString(), ex);
            }
        }

        public static IEnumerable<TextEntityDescription> ParseText(
            string text, IEnumerable<TwitterEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            var escaped = ParsingExtension.EscapeEntity(text);
            var endIndex = 0;

            // distinct by start_index ignores extended_entities.
            var lastIndex = -1;
            foreach (var entity in entities.OrderBy(e => e.Indices.Item1))
            {
                // the entity that has same start-index of previous entity should be ignored.
                if (entity.Indices.Item1 == lastIndex) continue;
                lastIndex = entity.Indices.Item1;

                if (endIndex < entity.Indices.Item1)
                {
                    // return raw string
                    yield return new TextEntityDescription(ParsingExtension.ResolveEntity(
                        escaped.SurrogatedSubstring(endIndex, entity.Indices.Item1 - endIndex)));
                }
                // get entitied text
                var body = ParsingExtension.ResolveEntity(escaped.SurrogatedSubstring(
                    entity.Indices.Item1, entity.Indices.Item2 - entity.Indices.Item1));
                yield return new TextEntityDescription(body, entity);
                endIndex = entity.Indices.Item2;
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
    }

    public class TextEntityDescription
    {
        public TextEntityDescription(string text, TwitterEntity entity = null)
        {
            Text = text;
            Entity = entity;
        }

        public string Text { get; }

        [CanBeNull]
        public TwitterEntity Entity { get; }
    }
}