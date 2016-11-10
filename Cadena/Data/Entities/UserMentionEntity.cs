using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena.Data.Entities
{
    public class UserMentionEntity : TwitterEntity
    {
        public UserMentionEntity(JsonValue json) : base(json)
        {
            Id = json["id"].AsLong();
            ScreenName = json["screen_name"].AsStringOrNull();
            Name = json["name"].AsStringOrNull();
            FullText = DisplayText = "@" + ScreenName;
        }

        /// <summary>
        /// Display text, contains @screen_name
        /// </summary>
        public override string DisplayText { get; }

        /// <summary>
        /// Full text, equals to display text, contains @screen_name
        /// </summary>
        public override string FullText { get; }

        /// <summary>
        /// Target user ID
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Target user screen_name (not contains @ mark)
        /// </summary>
        [CanBeNull]
        public string ScreenName { get; }

        /// <summary>
        /// Target user's name (this element is not an identifier)
        /// </summary>
        [CanBeNull]
        public string Name { get; }
    }
}
