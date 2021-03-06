﻿using System;
using System.Diagnostics;
using System.Linq;
using Cadena.Data;
using Cadena.Data.Streams;
using Cadena.Data.Streams.Events;
using Cadena.Data.Streams.Warnings;
using Cadena.Engine.StreamReceivers;
using Cadena.Meteor;
using Cadena.Util;

namespace Cadena.Engine._Internals.Parsers
{
    internal static class UserStreamParser
    {
        private const string EventSourceKey = "source";
        private const string EventTargetKey = "target";
        private const string EventCreatedAtKey = "target";
        private const string EventTargetObjectKey = "target_object";

        /// <summary>
        /// Parse streamed JSON line
        /// </summary>
        /// <param name="line">JSON line</param>
        /// <param name="handler">result handler</param>
        public static void ParseStreamLine(string line, IStreamHandler handler)
        {
            try
            {
                var element = MeteorJson.Parse(line);
                ParseStreamLine(element, handler);
            }
            catch (Exception ex)
            {
                handler.OnException(new StreamParseException(
                    "JSON parse failed.", line, ex));
            }
        }

        public static void ParseStreamLine(JsonStringParser parser, string line, IStreamHandler handler)
        {
            try
            {
                var element = parser.Parse(line);
                ParseStreamLine(element, handler);
            }
            catch (Exception ex)
            {
                handler.OnException(new StreamParseException(
                    "JSON parse failed.", line, ex));
            }
        }

        /// <summary>
        /// Parse streamed JSON line
        /// </summary>
        /// <param name="graph">JSON object graph</param>
        /// <param name="handler">result handler</param>
        public static void ParseStreamLine(JsonValue graph, IStreamHandler handler)
        {
            try
            {
                // element.foo() -> element.IsDefined("foo")

                //
                // fast path: first, identify standard status payload
                ////////////////////////////////////////////////////////////////////////////////////
                if (TwitterStreamParser.ParseStreamLineAsStatus(graph, handler))
                {
                    return;
                }

                //
                // parse stream-specific elements
                //

                // friends lists
                var friends = graph["friends"].AsArrayOrNull();
                if (friends != null)
                {
                    // friends enumeration
                    var friendsIds = friends.Select(v => v.AsLong()).ToArray();
                    handler.OnMessage(new StreamEnumeration(friendsIds));
                    return;
                }
                friends = graph["friends_str"].AsArrayOrNull();
                if (friends != null)
                {
                    // friends enumeration(stringified)
                    var friendsIds = friends.Select(v => v.AsString().ParseLong()).ToArray();
                    handler.OnMessage(new StreamEnumeration(friendsIds));
                    return;
                }

                var @event = graph["event"].AsStringOrNull();
                if (@event != null)
                {
                    ParseStreamEvent(@event.ToLower(), graph, handler);
                    return;
                }

                // too many follows warning
                var warning = graph["warning"].AsObjectOrNull();
                if (warning != null)
                {
                    var code = warning["code"].AsString();
                    if (code == "FOLLOWS_OVER_LIMIT")
                    {
                        handler.OnMessage(new StreamTooManyFollowsWarning(
                            code,
                            warning["message"].AsString(),
                            warning["user_id"].AsLong(),
                            TwitterStreamParser.GetTimestamp(warning)));
                        return;
                    }
                }

                // fallback to default stream handler
                TwitterStreamParser.ParseNotStatusStreamLine(graph, handler);
            }
            catch (Exception ex)
            {
                handler.OnException(new StreamParseException(
                    "Stream graph parse failed.", graph.ToString(), ex));
            }
        }

        /// <summary>
        /// Parse streamed twitter event
        /// </summary>
        /// <param name="ev">event name</param>
        /// <param name="graph">JSON object graph</param>
        /// <param name="handler">result handler</param>
        private static void ParseStreamEvent(string ev, JsonValue graph, IStreamHandler handler)
        {
            try
            {
                var source = new TwitterUser(graph[EventSourceKey]);
                var target = new TwitterUser(graph[EventTargetKey]);
                var timestamp = graph[EventCreatedAtKey].AsString().ParseTwitterDateTime();
                switch (ev)
                {
                    case StreamStatusEvent.FavoriteEventKey:
                    case StreamStatusEvent.UnfavoriteEventKey:
                    case StreamStatusEvent.QuotedTweetEventKey:
                    case StreamStatusEvent.FavoritedRetweetEventKey:
                    case StreamStatusEvent.RetweetedRetweetEventKey:
                        handler.OnMessage(new StreamStatusEvent(source, target,
                            new TwitterStatus(graph[EventTargetObjectKey]), ev, timestamp));
                        break;

                    case StreamUserEvent.BlockEventKey:
                    case StreamUserEvent.UnblockEventKey:
                    case StreamUserEvent.FollowEventKey:
                    case StreamUserEvent.UnfollowEventKey:
                    case StreamUserEvent.UserMuteEventKey:
                    case StreamUserEvent.UserUnmuteEventKey:
                    case StreamUserEvent.UserUpdateEventKey:
                    case StreamUserEvent.UserDeleteEventKey:
                    case StreamUserEvent.UserSuspendEventKey:
                        handler.OnMessage(new StreamUserEvent(source, target,
                            ev, timestamp));
                        break;

                    case StreamListEvent.ListCreatedEventKey:
                    case StreamListEvent.ListDestroyedEventKey:
                    case StreamListEvent.ListUpdatedEventKey:
                    case StreamListEvent.ListMemberAddedEventKey:
                    case StreamListEvent.ListMemberRemovedEventKey:
                    case StreamListEvent.ListUserSubscribedEventKey:
                    case StreamListEvent.ListUserUnsubscribedEventKey:
                        handler.OnMessage(new StreamListEvent(source, target,
                            new TwitterList(graph[EventTargetObjectKey]), ev, timestamp));
                        break;

                    case StreamAccessInformationEvent.AccessRevokedEventKey:
                    case StreamAccessInformationEvent.AccessUnrevokedEventKey:
                        handler.OnMessage(new StreamAccessInformationEvent(source, target,
                            new AccessInformation(graph[EventTargetObjectKey]), ev, timestamp));
                        break;
                    default:
                        Debug.WriteLine("unknown event: " + ev);
                        break;
                }
            }
            catch (Exception ex)
            {
                handler.OnException(new StreamParseException(
                    "Event parse failed:" + ev, graph.ToString(), ex));
            }
        }
    }
}