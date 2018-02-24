using System;
using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena.Data.Entities
{
    public sealed class TwitterHashtagEntity : TwitterEntity
    {
        public TwitterHashtagEntity(JsonValue json) : base(json)
        {
            Text = json["text"].AsStringOrNull()?.Replace("#", "");
        }

        public TwitterHashtagEntity(Tuple<int, int> indices, string text) : base(indices)
        {
            Text = text?.Replace("#", "");
        }

        /// <summary>
        /// Represents full text, equals to display text, #hashtag
        /// </summary>
        public override string FullText => "#" + Text;

        /// <summary>
        /// Represents hashtag text (not contains # mark)
        /// </summary>
        [CanBeNull]
        public string Text { get; }
    }
}