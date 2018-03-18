using System;

namespace Cadena.Data.Streams.Events
{
    /// <summary>
    /// List events
    /// </summary>
    /// <remarks>
    /// This message indicates: events about twitter lists
    ///
    /// This element is supported by: user streams, site streams
    /// </remarks>
    public sealed class StreamListEvent : StreamEvent<TwitterList, ListEvents>
    {
        internal const string ListCreatedEventKey = "list_created";
        internal const string ListDestroyedEventKey = "list_destroyed";
        internal const string ListUpdatedEventKey = "list_updated";
        internal const string ListMemberAddedEventKey = "list_member_added";
        internal const string ListMemberRemovedEventKey = "list_member_removed";
        internal const string ListUserSubscribedEventKey = "list_user_subscribed";
        internal const string ListUserUnsubscribedEventKey = "list_user_unsubscribed";

        public StreamListEvent(TwitterUser source, TwitterUser target,
            TwitterList targetObject, string rawEvent, DateTime createdAt)
            : base(source, target, targetObject, ToEnumEvent(rawEvent), rawEvent, createdAt)
        {
        }

        public static ListEvents ToEnumEvent(string eventStr)
        {
            switch (eventStr)
            {
                case ListCreatedEventKey:
                    return ListEvents.ListCreated;

                case ListDestroyedEventKey:
                    return ListEvents.ListDestroyed;

                case ListUpdatedEventKey:
                    return ListEvents.ListUpdated;

                case ListMemberAddedEventKey:
                    return ListEvents.ListMemberAdded;

                case ListMemberRemovedEventKey:
                    return ListEvents.ListMemberRemoved;

                case ListUserSubscribedEventKey:
                    return ListEvents.ListUserSubscribed;

                case ListUserUnsubscribedEventKey:
                    return ListEvents.ListUserUnsubscribed;

                default:
                    return ListEvents.Unknown;
            }
        }
    }

    public enum ListEvents
    {
        Unknown,
        ListCreated,
        ListDestroyed,
        ListUpdated,
        ListMemberAdded,
        ListMemberRemoved,
        ListUserSubscribed,
        ListUserUnsubscribed,
    }
}