﻿using System;
using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena.Data.Entities
{
    public sealed class TwitterUserMentionEntity : TwitterEntity
    {
        public TwitterUserMentionEntity(JsonValue json) : base(json)
        {
            Id = json["id"].AsLong();
            ScreenName = json["screen_name"].AsStringOrNull()?.Replace("@", "");
            Name = json["name"].AsStringOrNull();
        }

        public TwitterUserMentionEntity(Tuple<int, int> indices,
            long id, [CanBeNull] string screenName, [CanBeNull] string name)
            : base(indices)
        {
            Id = id;
            ScreenName = screenName?.Replace("@", "");
            Name = name;
        }

        /// <summary>
        /// Full text, equals to display text, contains @screen_name
        /// </summary>
        public override string FullText => "@" + ScreenName;

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