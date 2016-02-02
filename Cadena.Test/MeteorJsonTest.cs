using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Cadena.Meteor;
using Codeplex.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Cadena.Test
{
    [TestClass]
    public class MeteorJsonTest
    {
        [TestMethod]
        public void MeteorJsonDecodeTest()
        {
            try
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
                    // Trace.WriteLine(MeteorJson.Parse(elements).ToString());
                    Trace.WriteLine(MeteorJson.Parse(elements).ToString());
                }

            }
            catch (JsonParseException ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
        }

        [TestMethod]
        public void MeteorJsonPerformanceTest()
        {
            var cs = "";
            for (int i = 0; i < 50; i++)
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
                    var parsed = MeteorJson.Parse(elements);
                    if (parsed.ContainsKey("text") && parsed.ContainsKey("user") && parsed["user"].ContainsKey("screen_name"))
                    {
                        cs = "@" + parsed["user"]["screen_name"].GetString() + ": " + parsed["text"].GetString();
                    }
                }
            }
            // Trace.WriteLine(cs);
        }

        [TestMethod]
        public void DynamicJsonPerformanceTest()
        {
            var cs = "";
            for (int i = 0; i < 50; i++)
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
                    var parsed = DynamicJson.Parse(elements);
                    if (parsed.text() && parsed.user() && parsed.user.screen_name())
                    {
                        cs = (string)("@" + parsed.user.screen_name + ": " + parsed.text);
                    }
                }
            }
            // Trace.WriteLine(cs);
        }

        [TestMethod]
        public void NewtonsoftJsonPerformanceTest()
        {
            var cs = "";
            for (int i = 0; i < 50; i++)
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
                    var parsed = JObject.Parse(elements);
                    if (parsed["text"] != null && parsed["user"]?["screen_name"] != null)
                    {
                        cs = "@" + parsed["user"]["screen_name"] + ": " + parsed["text"];
                    }
                }
            }
            // Trace.WriteLine(cs);
        }


        [TestMethod]
        public void SystemTextJsonPerformanceTest()
        {
            var cs = "";
            for (int i = 0; i < 50; i++)
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
                    var parser = new JsonParser();
                    var parsed = parser.Parse(elements) as Dictionary<string, object>;
                    if (parsed != null && parsed.ContainsKey("text") && parsed.ContainsKey("user") && ((Dictionary<string, object>)parsed["user"]).ContainsKey("screen_name"))
                    {
                        var sn = (Dictionary<string, object>)parsed["user"];
                        cs = "@" + sn["screen_name"] + ": " + parsed["text"];
                    }
                }
            }
            // Trace.WriteLine(cs);
        }

        [TestMethod]
        public void JsonPerformanceTests()
        {
            var sw = new Stopwatch();

            DynamicJsonPerformanceTest();
            sw.Start();
            DynamicJsonPerformanceTest();
            sw.Stop();
            Trace.WriteLine($"DynamicJson: {sw.ElapsedMilliseconds} ms.");
            sw.Reset();

            SystemTextJsonPerformanceTest();
            sw.Start();
            SystemTextJsonPerformanceTest();
            sw.Stop();
            Trace.WriteLine($"System.Text.Json: {sw.ElapsedMilliseconds} ms.");
            sw.Reset();

            MeteorJsonPerformanceTest();
            sw.Start();
            MeteorJsonPerformanceTest();
            sw.Stop();
            Trace.WriteLine($"MeteorJson: {sw.ElapsedMilliseconds} ms.");
            sw.Reset();

            NewtonsoftJsonPerformanceTest();
            sw.Start();
            NewtonsoftJsonPerformanceTest();
            sw.Stop();
            Trace.WriteLine($"Newtonsoft.Json: {sw.ElapsedMilliseconds} ms.");
            sw.Reset();
        }
    }
}
