﻿using System;
using JetBrains.Annotations;

namespace Cadena.Data
{
    /// <summary>
    /// Represents friendship between two users
    /// </summary>
    public class TwitterFriendship
    {
        public TwitterFriendship(dynamic json)
        {
            var rel = json.relationship;
            var src = rel.source;
            var tgt = rel.target;
            SourceId = Int64.Parse(src.id_str);
            SourceScreenName = src.screen_name;
            TargetId = Int64.Parse(tgt.id_str);
            TargetScreenName = tgt.screen_name;
            IsSourceFollowingTarget = src.following;
            IsTargetFollowingSource = src.followed_by;
            IsBlocking = src.blocking;
            IsMuting = src.muting;
            // if source is not following target, twitter always returns false.
            IsWantRetweets = IsSourceFollowingTarget ? ((bool?)src.want_retweets) : null;
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