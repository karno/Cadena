using System;
using Cadena.Meteor;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena.Data
{
    /// <summary>
    /// Represents friendship between two users
    /// </summary>
    public class TwitterFriendship
    {
        public TwitterFriendship(JsonValue json)
        {
            var rel = json["relationship"];
            var src = rel["source"];
            var tgt = rel["target"];
            SourceId = src["id_str"].AsString().ParseLong();
            SourceScreenName = src["screen_name"].AsString() ?? String.Empty;
            TargetId = tgt["id_str"].AsString().ParseLong();
            TargetScreenName = tgt["screen_name"].AsString() ?? String.Empty;
            IsSourceFollowingTarget = src["following"].AsBoolean();
            IsTargetFollowingSource = src["followed_by"].AsBoolean();
            IsBlocking = src["blocking"].AsBoolean();
            IsMuting = src["muting"].AsBoolean();
            // if source is not following target, twitter always returns false.
            IsWantRetweets = IsSourceFollowingTarget ? (bool?)src["want_retweets"].AsBoolean() : null;
            if (SourceScreenName == null)
            {
                throw new ArgumentException("source.screen_name could not be null.");
            }
            if (TargetScreenName == null)
            {
                throw new ArgumentException("target.screen_name could not be null.");
            }
        }

        /// <summary>
        /// Source user id
        /// </summary>
        public long SourceId { get; }

        /// <summary>
        /// Source user screen name
        /// </summary>
        [NotNull]
        public string SourceScreenName { get; }

        /// <summary>
        /// Target user id
        /// </summary>
        public long TargetId { get; }

        /// <summary>
        /// Target user screen name
        /// </summary>
        [NotNull]
        public string TargetScreenName { get; }

        /// <summary>
        /// Source user following target
        /// </summary>
        public bool IsSourceFollowingTarget { get; }

        /// <summary>
        /// Target user following source
        /// </summary>
        public bool IsTargetFollowingSource { get; }

        /// <summary>
        /// Source user is blocking target
        /// </summary>
        public bool IsBlocking { get; }

        /// <summary>
        /// Source user is muting target
        /// </summary>
        public bool? IsMuting { get; }

        /// <summary>
        /// Source user is welcomes target's retweets
        /// </summary>
        public bool? IsWantRetweets { get; }
    }
}
