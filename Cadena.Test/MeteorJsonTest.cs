using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Cadena.Meteor;
using Codeplex.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cadena.Test
{
    [TestClass]
    public class MeteorJsonTest
    {
        [TestMethod]
        public void UnescapePerformanceTest()
        {
            int count = 100000;
            var sw = new Stopwatch();
            sw.Start();
            foreach (var sample in TweetSamples.GetStreamSamples())
            {
                // MeteorJsonHelper.Unescape(sample, sample.Length);
                MeteorJsonHelper.UnsafeUnescape(sample, sample.Length);
                count--;
                if (count == 0) break;
            }
            sw.Stop();
            Trace.WriteLine("Elapsed: " + sw.ElapsedMilliseconds);
        }

        [TestMethod]
        public void MeteorJsonDecodeTest()
        {
            try
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
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
            for (int i = 0; i < 50; i++)
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
                    var parsed = MeteorJson.Parse(elements);
                    if (parsed.ContainsKey("text") && parsed.ContainsKey("user") && parsed["user"].ContainsKey("screen_name"))
                    {
                        Trace.WriteLine("@" + parsed["user"]["screen_name"].GetString() + ": " + parsed["text"].GetString());
                    }
                }
            }
        }

        [TestMethod]
        public void DynamicJsonPerformanceTest()
        {
            for (int i = 0; i < 50; i++)
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
                    var parsed = DynamicJson.Parse(elements);
                    if (parsed.text() && parsed.user() && parsed.user.screen_name())
                    {
                        Trace.WriteLine((string)("@" + parsed.user.screen_name + ": " + parsed.text));
                    }
                }
            }
        }

        [TestMethod]
        public void JsonReaderWriterFactoryPerformanceTest()
        {
            for (int i = 0; i < 50; i++)
            {
                foreach (var elements in TweetSamples.GetStreamSampleElements())
                {
                    using (var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(elements),
                            XmlDictionaryReaderQuotas.Max))
                    {
                        var parsed = XElement.Load(reader);
                    }
                }
            }
        }

    }
}
