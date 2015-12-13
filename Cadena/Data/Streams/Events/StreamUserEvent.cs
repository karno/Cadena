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
        public StreamUserEvent(TwitterUser source, TwitterUser target,
            string rawEvent, DateTime createdAt)
            : base(source, target, target, ToEnumEvent(rawEvent), rawEvent, createdAt)
        { }

        public static UserEvents ToEnumEvent(string eventStr)
        {
            switch (eventStr.ToLower())
            {
                case "block":
                    return UserEvents.Block;
                case "unblock":
                    return UserEvents.Unblock;
                case "follow":
                    return UserEvents.Follow;
                case "unfollow":
                    return UserEvents.Unfollow;
                case "user_update":
                    return UserEvents.UserUpdate;
                case "user_delete":
                    return UserEvents.UserDelete;
                case "user_suspend":
                    return UserEvents.UserSuspend;
                case "mute":
                    return UserEvents.Mute;
                case "unmute":
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
