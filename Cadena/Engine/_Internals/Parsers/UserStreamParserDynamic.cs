using System;
using System.Linq;
using Cadena.Data;
using Cadena.Data.Streams;
using Cadena.Data.Streams.Events;
using Cadena.Data.Streams.Warnings;
using Cadena.Engine.StreamReceivers;
using Cadena.Util;
using Codeplex.Data;

namespace Cadena.Engine._Internals.Parsers
{
    internal static class UserStreamParserDynamic
    {

        /// <summary>
        /// Parse streamed JSON line
        /// </summary>
        /// <param name="line">JSON line</param>
        /// <param name="handler">result handler</param>
        public static void ParseStreamLine(string line, IStreamHandler handler)
        {
            try
            {
                var element = DynamicJson.Parse(line);
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
        internal static void ParseStreamLine(dynamic graph, IStreamHandler handler)
        {
            try
            {
                // element.foo() -> element.IsDefined("foo")

                //
                // fast path: first, identify standard status payload
                ////////////////////////////////////////////////////////////////////////////////////
                if (TwitterStreamParserDynamic.ParseStreamLineAsStatus(graph, handler))
                {
                    return;
                }

                //
                // parse stream-specific elements
                //

                // friends lists
                if (graph.friends())
                {
                    // friends enumeration
                    handler.OnMessage(new StreamEnumeration((long[])graph.friends));
                    return;
                }
                if (graph.friends_str())
                {
                    // friends enumeration(stringified)
                    handler.OnMessage(new StreamEnumeration(
                        ((string[])graph.friends).Select(s => s.ParseLong()).ToArray()));
                    return;
                }

                if (graph.IsDefined("event")) // graph.event()
                {
                    ParseStreamEvent(((string)graph["event"]).ToLower(), graph, handler);
                    return;
                }


                // too many follows warning
                if (graph.warning())
                {
                    if (graph.warning.code == "FOLLOWS_OVER_LIMIT")
                    {
                        handler.OnMessage(new StreamTooManyFollowsWarning(
                            graph.warning.code,
                            graph.warning.message,
                            graph.warning.user_id,
                            TwitterStreamParserDynamic.GetTimestamp(graph.warning)));
                        return;
                    }
                }

                // fallback to default stream handler
                TwitterStreamParserDynamic.ParseNotStatusStreamLine(graph, handler);
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
        private static void ParseStreamEvent(string ev, dynamic graph, IStreamHandler handler)
        {
            try
            {
                var source = new TwitterUser(graph.source);
                var target = new TwitterUser(graph.target);
                var timestamp = ((string)graph.created_at).ParseTwitterDateTime();
                switch (ev)
                {
                    case "favorite":
                    case "unfavorite":
                    case "quoted_tweet":
                    case "favorited_retweet":
                    case "retweeted_retweet":
                        handler.OnMessage(new StreamStatusEvent(source, target,
                            new TwitterStatus(graph.target_object), ev, timestamp));
                        break;
                    case "block":
                    case "unblock":
                    case "follow":
                    case "unfollow":
                    case "mute":
                    case "unmute":
                    case "user_update":
                    case "user_delete":
                    case "user_suspend":
                        handler.OnMessage(new StreamUserEvent(source, target,
                            ev, timestamp));
                        break;
                    case "list_created":
                    case "list_destroyed":
                    case "list_updated":
                    case "list_member_added":
                    case "list_member_removed":
                    case "list_user_subscribed":
                    case "list_user_unsubscribed":
                        handler.OnMessage(new StreamListEvent(source, target,
                            new TwitterList(graph.target_object), ev, timestamp));
                        break;
                    case "access_revoked":
                    case "access_unrevoked":
                        handler.OnMessage(new StreamAccessInformationEvent(source, target,
                            new AccessInformation(graph.target_object), ev, timestamp));
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
