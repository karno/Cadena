using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena.Data.Entities
{
    public sealed class SymbolEntity : TwitterEntity
    {
        public SymbolEntity(JsonValue json) : base(json)
        {
            Text = json["text"].AsStringOrNull();
            FullText = DisplayText = "$" + Text;
        }

        /// <summary>
        /// Represents display text, $symbol
        /// </summary>
        public override string DisplayText { get; }

        /// <summary>
        /// Represents full text, equals to display text, $symbol
        /// </summary>
        public override string FullText { get; }

        /// <summary>
        /// Symbol text
        /// </summary>
        [CanBeNull]
        public string Text { get; }
    }
}
