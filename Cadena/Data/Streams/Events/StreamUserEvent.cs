using System;

namespace Cadena.Data.Streams.Events
{
    /// <summary>
    /// User events
    /// </summary>
    /// <remarks>
    /// This message indicates: events about twitter users
    ///
    /// This element is supported by: user streams, site streams
    /// (Except: user_update is also used in the (generic) streams)
    /// </remarks>
    public sealed class StreamUserEvent : StreamEvent<TwitterUser, UserEvents>
    {
        internal const string BlockEventKey = "block";
        internal const string UnblockEventKey = "unblock";
        internal const string FollowEventKey = "follow";
        internal const string UnfollowEventKey = "unfollow";
        internal const string UserUpdateEventKey = "user_update";
        internal const string UserDeleteEventKey = "user_delete";
        internal const string UserSuspendEventKey = "user_suspend";
        internal const string UserMuteEventKey = "mute";
        internal const string UserUnmuteEventKey = "unmute";

        public StreamUserEvent(TwitterUser source, TwitterUser target,
            string rawEvent, DateTime createdAt)
            : base(source, target, target, ToEnumEvent(rawEvent), rawEvent, createdAt)
        {
        }

        public static UserEvents ToEnumEvent(string eventStr)
        {
            switch (eventStr.ToLower())
            {
                case BlockEventKey:
                    return UserEvents.Block;

                case UnblockEventKey:
                    return UserEvents.Unblock;

                case FollowEventKey:
                    return UserEvents.Follow;

                case UnfollowEventKey:
                    return UserEvents.Unfollow;

                case UserUpdateEventKey:
                    return UserEvents.UserUpdate;

                case UserDeleteEventKey:
                    return UserEvents.UserDelete;

                case UserSuspendEventKey:
                    return UserEvents.UserSuspend;

                case UserMuteEventKey:
                    return UserEvents.Mute;

                case UserUnmuteEventKey:
                    return UserEvents.UnMute;

                default:
                    return UserEvents.Unknown;
            }
        }
    }

    public enum UserEvents
    {
        Unknown = -1,
        Follow,
        Unfollow,
        Block,
        Unblock,
        Mute,
        UnMute,
        UserUpdate,
        UserDelete,
        UserSuspend,
    }
}