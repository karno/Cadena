﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Cadena.Meteor;
using Cadena.Meteor.Safe;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cadena.Test
{
    [TestClass]
    public class MeteorJsonTest
    {
        const int LoopCount = 100;

        [TestMethod]
        public void MeteorJsonNullBehaviorTest()
        {
            Assert.AreEqual(JsonNull.Null.Equals(null), true);
            Assert.AreEqual(JsonNull.Null.Equals(new object()), false);
            Assert.AreEqual(JsonNull.Null == null, true);
            Assert.AreEqual(JsonNull.Null != null, false);
            Assert.AreEqual(null == JsonNull.Null, true);
            Assert.AreEqual(null != JsonNull.Null, false);
        }

        [TestMethod]
        public void MeteorJsonNumericBehaviorTest()
        {
            Assert.AreEqual(MeteorJson.Parse("1234567890e0").Equals(1234567890e0), true);
            Assert.AreEqual(MeteorJson.Parse("1.2345").Equals(1.2345), true);
        }


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
                    Debug.WriteLine("{0}", MeteorJson.Parse(elements).ToString());
                    Debug.WriteLine("{0}", SafeMeteorJson.Parse(elements).ToString());
                }

            }
            catch (JsonParseException ex)
            {
                Debug.WriteLine(ex.ToString());
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
                    var reader = new JsonStreamParser(memstream);
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
                    var reader = new SafeJsonStreamParser(memstream);
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
        public void JsonPerformanceTests()
        {
            RunPerformanceTest("MeteorJson", MeteorJsonPerformanceTest);
            RunPerformanceTest("MeteorJson(Safe)", SafeMeteorJsonPerformanceTest);
            RunPerformanceTest("MeteorJson(Stream)", MeteorJsonStreamPerformanceTest);
            RunPerformanceTest("MeteorJson(SafeStream)", SafeMeteorJsonStreamPerformanceTest);
        }

        private void RunPerformanceTest(string name, Action testFunc)
        {
            var proc = Process.GetCurrentProcess();
            var sw = new Stopwatch();
            // pre-run
            testFunc();
            // measure start
            var mb = proc.PrivateMemorySize64;
            sw.Start();
            // main run
            testFunc();
            // measure stop
            sw.Stop();
            var ma = proc.PrivateMemorySize64;
            var delta = (ma - mb) / 1024;
            // print result
            Debug.WriteLine("{0}: {1}ms, delta:{2}KB ({3} -> {4})", name, sw.ElapsedMilliseconds, delta, mb, ma);


        }
    }
}
