using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena.Data.Entities
{
    public sealed class HashtagEntity : TwitterEntity
    {
        public HashtagEntity(JsonValue json) : base(json)
        {
            Text = json["text"].AsStringOrNull();
            FullText = DisplayText = "#" + Text;
        }

        /// <summary>
        /// Represents display text, #hashtag
        /// </summary>
        public override string DisplayText { get; }

        /// <summary>
        /// Represents full text, equals to display text, #hashtag
        /// </summary>
        public override string FullText { get; }

        /// <summary>
        /// Represents hashtag text (not contains # mark)
        /// </summary>
        [CanBeNull]
        public string Text { get; }
    }
}
