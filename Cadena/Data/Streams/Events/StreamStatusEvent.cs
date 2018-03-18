using System;

namespace Cadena.Data.Streams.Events
{
    /// <summary>
    /// Status events
    /// </summary>
    /// <remarks>
    /// This message indicates: events about twitter statuses
    ///
    /// This element is supported by: user streams, site streams
    /// </remarks>
    public sealed class StreamStatusEvent : StreamEvent<TwitterStatus, StatusEvents>
    {
        internal const string FavoriteEventKey = "favorite";
        internal const string UnfavoriteEventKey = "unfavorite";
        internal const string QuotedTweetEventKey = "quoted_tweet";
        internal const string FavoritedRetweetEventKey = "favorited_retweet";
        internal const string RetweetedRetweetEventKey = "retweeted_retweet";

        public StreamStatusEvent(TwitterUser source, TwitterUser target,
            TwitterStatus targetObject, string rawEvent, DateTime createdAt)
            : base(source, target, targetObject, ToEnumEvent(rawEvent), rawEvent, createdAt)
        {
        }

        private static StatusEvents ToEnumEvent(string eventStr)
        {
            switch (eventStr.ToLower())
            {
                case FavoriteEventKey:
                    return StatusEvents.Favorite;

                case UnfavoriteEventKey:
                    return StatusEvents.Unfavorite;

                case QuotedTweetEventKey:
                    return StatusEvents.Quote;

                case FavoritedRetweetEventKey:
                    return StatusEvents.FavoriteRetweet;

                case RetweetedRetweetEventKey:
                    return StatusEvents.RetweetRetweet;

                default:
                    return StatusEvents.Unknown;
            }
        }
    }

    public enum StatusEvents
    {
        Unknown = -1,
        Favorite,
        Unfavorite,
        FavoriteRetweet,
        RetweetRetweet,
        Quote,
    }
}