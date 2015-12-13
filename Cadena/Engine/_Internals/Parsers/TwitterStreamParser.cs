using System;
using System.Collections.Generic;
using Cadena.Data;
using Cadena.Data.Streams;
using Cadena.Data.Streams.Events;
using Cadena.Data.Streams.Warnings;
using Cadena.Engine.StreamReceivers;
using Cadena.Util;
using Codeplex.Data;

namespace Cadena.Engine._Internals.Parsers
{
    internal static class TwitterStreamParser
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
                if (!ParseStreamLineAsStatus(element, handler))
                {
                    ParseNotStatusStreamLine(element, handler);
                }
            }
            catch (Exception ex)
            {
                handler.OnException(new StreamParseException(
                    "JSON parse failed.", line, ex));
            }
        }

        /// <summary>
        /// Check parse streamed JSON line as normal (not direct-message) status
        /// </summary>
        /// <param name="graph">JSON object graph</param>
        /// <param name="handler">stream handler</param>
        /// <returns></returns>
        internal static bool ParseStreamLineAsStatus(dynamic graph, IStreamHandler handler)
        {
            if (!graph.text()) return false;
            handler.OnStatus(new TwitterStatus(graph));
            return true;
        }

        /// <summary>
        /// Parse streamed JSON line (which is not a status)
        /// </summary>
        /// <param name="graph">JSON object graph</param>
        /// <param name="handler">result handler</param>
        internal static void ParseNotStatusStreamLine(dynamic graph, IStreamHandler handler)
        {
            try
            {
                // element.foo() -> element.IsDefined("foo")

                // direct message
                if (graph.direct_message())
                {
                    handler.OnStatus(new TwitterStatus(graph.direct_message));
                    return;
                }

                // delete
                if (graph.delete())
                {
                    var timestamp = GetTimestamp(graph.delete);
                    if (graph.delete.status())
                    {
                        handler.OnMessage(new StreamDelete(
                            Int64.Parse(graph.delete.status.id_str),
                            Int64.Parse(graph.delete.status.user_id_str),
                            timestamp));
                        return;
                    }
                    if (graph.delete.direct_message())
                    {
                        handler.OnMessage(new StreamDelete(
                            Int64.Parse(graph.delete.status.id_str),
                            Int64.Parse(graph.delete.direct_message.user_id.ToString()),
                            timestamp));
                        return;
                    }
                }

                // scrub_geo
                if (graph.scrub_geo())
                {
                    handler.OnMessage(new StreamScrubGeo(
                        Int64.Parse(graph.scrub_geo.user_id_str),
                        Int64.Parse(graph.scrub_geo.up_to_status_id_str),
                        GetTimestamp(graph.scrub_geo)));
                    return;
                }

                // limit
                if (graph.limit())
                {
                    handler.OnMessage(new StreamLimit(
                        (long)graph.limit.track,
                        GetTimestamp(graph.limit)));
                    return;
                }

                // withheld
                if (graph.status_withheld())
                {
                    handler.OnMessage(new StreamWithheld(
                        Int64.Parse(graph.status_withheld.user_id),
                        Int64.Parse(graph.status_withheld.id),
                        graph.status_withheld.withheld_in_countries,
                        GetTimestamp(graph.status_withheld)));
                    return;
                }
                if (graph.user_withheld())
                {
                    handler.OnMessage(new StreamWithheld(
                        Int64.Parse(graph.user_withheld.id),
                        graph.user_withheld.withheld_in_countries,
                        GetTimestamp(graph.user_withheld)));
                    return;
                }

                // disconnect
                if (graph.disconnect())
                {
                    handler.OnMessage(new StreamDisconnect(
                        (DisconnectCode)graph.disconnect.code,
                        graph.disconnect.stream_name, graph.disconnect.reason,
                        GetTimestamp(graph.disconnect)));
                    return;
                }

                // stall warning
                if (graph.warning())
                {
                    var timestamp = GetTimestamp(graph.warning);
                    if (graph.warning.code == "FALLING_BEHIND")
                    {
                        handler.OnMessage(new StreamStallWarning(
                            graph.warning.code,
                            graph.warning.message,
                            graph.warning.percent_full,
                            timestamp));
                        return;
                    }
                }

                // user update
                if (graph.IsDefined("event")) // 'event' is the reserved word...
                {
                    var ev = ((string)graph["event"]).ToLower();
                    if (ev == "user_update")
                    {
                        // parse user_update only in generic streams.
                        handler.OnMessage(new StreamUserEvent(
                            new TwitterUser(graph.source),
                            new TwitterUser(graph.target), ev,
                            ((string)graph.created_at).ParseTwitterDateTime()));
                        return;
                    }
                    // unknown event...
                    handler.OnMessage(new StreamUnknownMessage("event: " + ev, graph.ToString()));
                }

                if (graph.IsObject())
                {
                    // unknown...
                    foreach (KeyValuePair<string, dynamic> item in graph)
                    {
                        handler.OnMessage(new StreamUnknownMessage(item.Key, item.Value.ToString()));
                        return;
                    }
                }
                // unknown event-type...
                handler.OnMessage(new StreamUnknownMessage(null, graph.Value.ToString()));

            }
            catch (Exception ex)
            {
                handler.OnException(new StreamParseException(
                    "Stream graph parse failed.", graph.ToString(), ex));
            }
        }

        /// <summary>
        /// Get timestamp_ms field or pseudo timestamp string.
        /// </summary>
        /// <param name="graph">json object graph</param>
        /// <returns>timestamp code(millisec)</returns>
        internal static string GetTimestamp(dynamic graph)
        {
            return graph.timestamp_ms()
                ? graph.timestamp_ms
                : ((long)(DateTime.Now.ToUniversalTime() - StreamMessage.SerialTime)
                    .TotalMilliseconds).ToString();
        }
    }
}
