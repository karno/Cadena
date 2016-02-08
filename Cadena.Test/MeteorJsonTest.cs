using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using Cadena.Meteor;
using Cadena.Meteor.Safe;
using Codeplex.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Cadena.Test
{
    [TestClass]
    public class MeteorJsonTest
    {
        const int LoopCount = 100;

        [TestMethod]
        public void MeteorJsonValueTest()
        {
            Assert.AreEqual(1234567890e0, MeteorJson.Parse("1234567890e0").AsDouble());
            Assert.AreEqual(1.234567890e4, MeteorJson.Parse("1.234567890e4").AsDouble(), 0.00000001);
            Assert.AreEqual(-543.21e-4, MeteorJson.Parse("-543.21e-4").AsDouble(), 0.0000001);
            Assert.AreEqual(
                "＼(#`・△・)/ < Merurulince Rede Arls!",
                MeteorJson.Parse(@"""＼(#`・△・)/ < Merurulince Rede Arls!""").AsString());
        }

        [TestMethod]
        public void MeteorJsonDecodeTest()
        {
            try
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
                    // Trace.WriteLine(MeteorJson.Parse(elements).ToString());
                    Trace.WriteLine(MeteorJson.Parse(elements).ToString());
                    Trace.WriteLine(SafeMeteorJson.Parse(elements).ToString());
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
            for (int i = 0; i < LoopCount; i++)
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
                    var parsed = MeteorJson.Parse(elements);
                    if (parsed.ContainsKey("text") && parsed.ContainsKey("user") && parsed["user"].ContainsKey("screen_name"))
                    {
                        cs = "@" + parsed["user"]["screen_name"].AsString() + ": " + parsed["text"].AsString();
                    }
                }
            }
            // Trace.WriteLine(cs);
        }

        [TestMethod]
        public void MeteorJsonStreamPerformanceTest()
        {
            var cs = "";
            for (int i = 0; i < LoopCount; i++)
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
                    var memstream = new MemoryStream(Encoding.UTF8.GetBytes(elements));
                    var reader = new JsonStreamReader(memstream);
                    var parsed = reader.Parse();
                    if (parsed.ContainsKey("text") && parsed.ContainsKey("user") && parsed["user"].ContainsKey("screen_name"))
                    {
                        cs = "@" + parsed["user"]["screen_name"].AsString() + ": " + parsed["text"].AsString();
                    }
                }
            }
            // Trace.WriteLine(cs);
        }

        [TestMethod]
        public void SafeMeteorJsonPerformanceTest()
        {
            var cs = "";
            for (int i = 0; i < LoopCount; i++)
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
                    var parsed = SafeMeteorJson.Parse(elements);
                    if (parsed.ContainsKey("text") && parsed.ContainsKey("user") && parsed["user"].ContainsKey("screen_name"))
                    {
                        cs = "@" + parsed["user"]["screen_name"].AsString() + ": " + parsed["text"].AsString();
                    }
                }
            }
            // Trace.WriteLine(cs);
        }

        [TestMethod]
        public void SafeMeteorJsonStreamPerformanceTest()
        {
            var cs = "";
            for (int i = 0; i < LoopCount; i++)
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
                    var memstream = new MemoryStream(Encoding.UTF8.GetBytes(elements));
                    var reader = new JsonSafeStreamReader(memstream);
                    var parsed = reader.Parse();
                    if (parsed.ContainsKey("text") && parsed.ContainsKey("user") && parsed["user"].ContainsKey("screen_name"))
                    {
                        cs = "@" + parsed["user"]["screen_name"].AsString() + ": " + parsed["text"].AsString();
                    }
                }
            }
            // Trace.WriteLine(cs);
        }

        [TestMethod]
        public void DynamicJsonPerformanceTest()
        {
            var cs = "";
            for (int i = 0; i < LoopCount; i++)
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
            for (int i = 0; i < LoopCount; i++)
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
            var parser = new JsonParser();
            for (int i = 0; i < LoopCount; i++)
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
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

            NewtonsoftJsonPerformanceTest();
            sw.Start();
            NewtonsoftJsonPerformanceTest();
            sw.Stop();
            Trace.WriteLine($"Newtonsoft.Json: {sw.ElapsedMilliseconds} ms.");
            sw.Reset();

            MeteorJsonPerformanceTest();
            sw.Start();
            MeteorJsonPerformanceTest();
            sw.Stop();
            Trace.WriteLine($"MeteorJson: {sw.ElapsedMilliseconds} ms.");
            sw.Reset();

            MeteorJsonPerformanceTest();
            sw.Start();
            SafeMeteorJsonPerformanceTest();
            sw.Stop();
            Trace.WriteLine($"MeteorJson(Safe): {sw.ElapsedMilliseconds} ms.");
            sw.Reset();

            MeteorJsonPerformanceTest();
            sw.Start();
            MeteorJsonStreamPerformanceTest();
            sw.Stop();
            Trace.WriteLine($"MeteorJson(Stream): {sw.ElapsedMilliseconds} ms.");
            sw.Reset();

            MeteorJsonPerformanceTest();
            sw.Start();
            SafeMeteorJsonStreamPerformanceTest();
            sw.Stop();
            Trace.WriteLine($"MeteorJson(SafeStream): {sw.ElapsedMilliseconds} ms.");
            sw.Reset();
        }
    }
}
