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
            var gif = File.ReadAllBytes("test.gif");
            var bmp = File.ReadAllBytes("test.bmp");
            var png = File.ReadAllBytes("test.png");
            var agif = File.ReadAllBytes("test_animated.gif");
            var mp4 = File.ReadAllBytes("test.mp4");
            Assert.AreEqual(MediaFileUtility.GetMediaType(gif), SupportedMediaTypes.Gif);
            Assert.AreEqual(MediaFileUtility.GetMediaType(bmp), SupportedMediaTypes.Bmp);
            Assert.AreEqual(MediaFileUtility.GetMediaType(png), SupportedMediaTypes.Png);
            Assert.AreEqual(MediaFileUtility.GetMediaType(agif), SupportedMediaTypes.AnimatedGif);
            Assert.AreEqual(MediaFileUtility.GetMediaType(mp4), SupportedMediaTypes.Mp4);
        }
    }
}
