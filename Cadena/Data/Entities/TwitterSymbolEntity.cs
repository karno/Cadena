using System;
using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena.Data.Entities
{
    public sealed class TwitterSymbolEntity : TwitterEntity
    {
        public TwitterSymbolEntity(JsonValue json) : base(json)
        {
            Text = json["text"].AsStringOrNull();
        }

        public TwitterSymbolEntity(Tuple<int, int> indices, [CanBeNull] string text) : base(indices)
        {
            Text = text;
        }

        /// <summary>
        /// Represents full text, equals to display text, $symbol
        /// </summary>
        public override string FullText => "$" + Text;

        /// <summary>
        /// Symbol text
        /// </summary>
        [CanBeNull]
        public string Text { get; }
    }
}