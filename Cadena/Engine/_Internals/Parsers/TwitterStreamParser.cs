using System;
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
                var element = MeteorJson.Parse(line);
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
        internal static bool ParseStreamLineAsStatus(JsonValue graph, IStreamHandler handler)
        {
            if (!graph.ContainsKey("text")) return false;
            handler.OnStatus(new TwitterStatus(graph));
            return true;
        }

        /// <summary>
        /// Parse streamed JSON line (which is not a status)
        /// </summary>
        /// <param name="graph">JSON object graph</param>
        /// <param name="handler">result handler</param>
        internal static void ParseNotStatusStreamLine(JsonValue graph, IStreamHandler handler)
        {
            try
            {
                // element.foo() -> element.IsDefined("foo")

                // direct message
                JsonValue directMessage;
                if (graph.TryGetValue("direct_message", out directMessage))
                {
                    handler.OnStatus(new TwitterStatus(graph["direct_message"]));
                    return;
                }

                // delete
                JsonValue delete;
                if (graph.TryGetValue("delete", out delete))
                {
                    var timestamp = GetTimestamp(delete);
                    JsonValue status;
                    if (delete.TryGetValue("status", out status))
                    {
                        handler.OnMessage(new StreamDelete(
                            Int64.Parse(status["id_str"].AsString()),
                            Int64.Parse(status["user_id_str"].AsString()),
                            timestamp));
                        return;
                    }
                    if (delete.TryGetValue("direct_message", out directMessage))
                    {
                        handler.OnMessage(new StreamDelete(
                            Int64.Parse(directMessage["id_str"].AsString()),
                            Int64.Parse(directMessage["user_id"].AsString()),
                            timestamp));
                        return;
                    }
                }

                // scrub_geo
                JsonValue scrubGeo;
                if (graph.TryGetValue("scrub_geo", out scrubGeo))
                {
                    handler.OnMessage(new StreamScrubGeo(
                        Int64.Parse(scrubGeo["user_id_str"].AsString()),
                        Int64.Parse(scrubGeo["up_to_status_id_str"].AsString()),
                        GetTimestamp(scrubGeo)));
                    return;
                }

                // limit
                JsonValue limit;
                if (graph.TryGetValue("limit", out limit))
                {
                    handler.OnMessage(new StreamLimit(
                        limit["track"].AsLong(),
                        GetTimestamp(limit)));
                    return;
                }

                // withheld
                JsonValue statusWithheld;
                if (graph.TryGetValue("status_withheld", out statusWithheld))
                {
                    handler.OnMessage(new StreamWithheld(
                        statusWithheld["user_id"].AsLong(),
                        statusWithheld["id"].AsLong(),
                        ((JsonArray)statusWithheld["withheld_in_countries"]).Select(s => s.AsString()).ToArray(),
                        GetTimestamp(statusWithheld)));
                    return;
                }
                JsonValue userWithheld;
                if (graph.TryGetValue("user_withheld", out userWithheld))
                {
                    handler.OnMessage(new StreamWithheld(
                        userWithheld["id"].AsLong(),
                        ((JsonArray)statusWithheld["withheld_in_countries"]).Select(s => s.AsString()).ToArray(),
                        GetTimestamp(statusWithheld)));
                    return;
                }

                // disconnect
                JsonValue disconnect;
                if (graph.TryGetValue("disconnect", out disconnect))
                {
                    handler.OnMessage(new StreamDisconnect(
                        (DisconnectCode)disconnect["code"].AsLong(),
                        disconnect["stream_name"].AsString(),
                        disconnect["reason"].AsString(),
                        GetTimestamp(disconnect)));
                    return;
                }

                // stall warning
                JsonValue warning;
                if (graph.TryGetValue("warning", out warning))
                {
                    var timestamp = GetTimestamp(warning);
                    var code = warning["code"].AsString();
                    if (code == "FALLING_BEHIND")
                    {
                        handler.OnMessage(new StreamStallWarning(
                            code,
                            warning["message"].AsString(),
                            (int)warning["percent_full"].AsLong(),
                            timestamp));
                        return;
                    }
                }

                // user update
                JsonValue @event;
                if (graph.TryGetValue("event", out @event))
                {
                    var ev = @event.AsString().ToLower();
                    if (ev == "user_update")
                    {
                        // parse user_update only in generic streams.
                        handler.OnMessage(new StreamUserEvent(
                            new TwitterUser(graph["source"]),
                            new TwitterUser(graph["target"]),
                            ev, graph["created_at"].AsString().ParseTwitterDateTime()));
                        return;
                    }
                    // unknown event...
                    handler.OnMessage(new StreamUnknownMessage("event: " + ev, graph.ToString()));
                }

                // unknown event-type...
                handler.OnMessage(new StreamUnknownMessage(null, graph.ToString()));

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
        internal static string GetTimestamp(JsonValue graph)
        {
            return graph.ContainsKey("timestamp_ms")
                ? graph["timestamp_ms"].AsString()
                : ((long)(DateTime.Now.ToUniversalTime() - StreamMessage.SerialTime)
                    .TotalMilliseconds).ToString();
        }
    }
}
