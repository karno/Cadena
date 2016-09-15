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
            Assert.AreEqual(MediaFileUtility.GetMediaType(gif), SupportedMediaTypes.Gif);
            Assert.AreEqual(MediaFileUtility.GetMediaType(bmp), SupportedMediaTypes.Bmp);
            Assert.AreEqual(MediaFileUtility.GetMediaType(png), SupportedMediaTypes.Png);
            Assert.AreEqual(MediaFileUtility.GetMediaType(agif), SupportedMediaTypes.AnimatedGif);
            Assert.AreEqual(MediaFileUtility.GetMediaType(mp4), SupportedMediaTypes.Mp4);
        }
    }
}
