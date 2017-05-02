using System.IO;
using Cadena.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cadena.Test
{
    [TestClass]
    public class MediaFileUtilTest
    {
        [TestMethod]
        public void DetermineImageType()
        {
            var dir = "..\\..\\TestMedia\\";
            var gif = File.ReadAllBytes(dir + "test.gif");
            var bmp = File.ReadAllBytes(dir + "test.bmp");
            var png = File.ReadAllBytes(dir + "test.png");
            var agif = File.ReadAllBytes(dir + "test_animated.gif");
            var mp4 = File.ReadAllBytes(dir + "test.mp4");
            Assert.AreEqual(SupportedMediaTypes.Gif, MediaFileUtility.GetMediaType(gif));
            Assert.AreEqual(SupportedMediaTypes.Bmp, MediaFileUtility.GetMediaType(bmp));
            Assert.AreEqual(SupportedMediaTypes.Png, MediaFileUtility.GetMediaType(png));
            Assert.AreEqual(SupportedMediaTypes.AnimatedGif, MediaFileUtility.GetMediaType(agif));
            Assert.AreEqual(SupportedMediaTypes.Mp4, MediaFileUtility.GetMediaType(mp4));
        }
    }
}