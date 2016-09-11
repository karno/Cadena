// Twitter has not supported WebP currently.
// but, their document indicates WebP is supported.
// https://dev.twitter.com/rest/reference/post/media/upload-chunked
// so, we suppress this feature currently but still can be activated it easily if supported.
// #define WEBP_SUPPORTED

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cadena.Util
{
    public static class MediaFileUtility
    {
        public const string MimeTypeBmp = "image/bmp";
        public const string MimeTypeGif = "image/gif";
        public const string MimeTypeJpeg = "image/jpeg";
        public const string MimeTypePng = "image/png";
        public const string MimeTypeMp4 = "video/mp4";
#if WEBP_SUPPORTED
        public const string MimeTypeWebP = "image/webp";
#endif

        public static SupportedMediaTypes GetMediaType(IEnumerable<byte> media)
        {
            var table = new Dictionary<SupportedMediaTypes, byte[]>
            {
                {SupportedMediaTypes.Bmp, Encoding.ASCII.GetBytes("BM")},
                {SupportedMediaTypes.Gif, Encoding.ASCII.GetBytes("GIF")},
                {SupportedMediaTypes.Jpeg, new byte[] {255, 216, 255, 224}},
                {SupportedMediaTypes.Png, new byte[] {137, 80, 78, 71}},
#if WEBP_SUPPORTED
                {SupportedMediaTypes.WebP, Encoding.ASCII.GetBytes("RIFF\0\0\0\0WEBP")},
#endif
                {SupportedMediaTypes.Mp4, Encoding.ASCII.GetBytes("\0\0\0\0ftyp")} // optimistic determination
            };

            var memoedMedia = media.Memoize();
            var headBytes = memoedMedia.Take(12).ToArray();
            if (CheckMediaIsAnimatedGif(memoedMedia))
            {
                return SupportedMediaTypes.AnimatedGif;
            }

            foreach (var b in media)
            {

            }

            foreach (var tuple in table)
            {
                var mediaType = tuple.Key;
                if (headBytes.Length < tuple.Value.Length)
                {
                    // too short to determine file type
                    continue;
                }
                if (tuple.Value.Where((t, i) => t != Convert.ToByte('\0') && t != headBytes[i]).Any())
                {
                    // type mismatched
                    mediaType = SupportedMediaTypes.Unknown;
                }
                if (mediaType != SupportedMediaTypes.Unknown)
                {
                    return mediaType;
                }
            }
            return SupportedMediaTypes.Unknown;
        }

        private static bool CheckMediaIsAnimatedGif(IEnumerable<byte> media)
        {
            // read bytes array
            var reader = new ByteEnumerationReader(media);
            // Animated GIF must have GIF89a header.
            if (!reader.GetNext(6).SequenceEqual(Encoding.ASCII.GetBytes("GIF89a"))) return false;
            // skip image size declaration (w, h, each 2 bytes)
            if (!reader.Skip(4))
                return false;
            // acquire color flag
            var flag = reader.GetNext(1);
            if (flag.Length != 1) return false;
            // calculate palette bytes length
            var paletteColors = 0;
            if ((flag[0] & 0x80) == 0x80) // 1000 0000 => global palette flag
            {
                paletteColors = 2 << (flag[0] & 0x07); // 0000 0111 => palette size, 2 << n
            }
            // skip reading background color, aspect ratio, and palette size
            // palette is 3 bytes for each color.
            if (!reader.Skip(1 + 1 + paletteColors * 3)) return false;
            // if this file is animated gif, the subsequent block must be an Application Extension block(0x21 0xff).
            // and, next element, block size (1 byte) is always 0x0b(11 bytes).
            if (!reader.GetNext(3).SequenceEqual(new byte[] { 0x21, 0xff, 0x0b })) return false;
            // application identifier and version must be "NETSCAPE2.0", the giant of good old-fashioned Web.
            if (!reader.GetNext(11).SequenceEqual(Encoding.ASCII.GetBytes("NETSCAPE2.0"))) return false;
            // completed. this file is must be Animated GIF. Thank you for your hard work.
            return true;
        }

        public static string GetMime(SupportedMediaTypes type)
        {
            switch (type)
            {
                case SupportedMediaTypes.Bmp:
                    return MimeTypeBmp;
                case SupportedMediaTypes.Gif:
                case SupportedMediaTypes.AnimatedGif:
                    return MimeTypeGif;
                case SupportedMediaTypes.Jpeg:
                    return MimeTypeJpeg;
                case SupportedMediaTypes.Png:
                    return MimeTypePng;
#if WEBP_SUPPORTED
                case SupportedMediaTypes.WebP:
                    return MimeTypeWebP;
#endif
                case SupportedMediaTypes.Mp4:
                    return MimeTypeMp4;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private sealed class ByteEnumerationReader
        {
            private readonly IEnumerator<byte> _enumerator;

            public ByteEnumerationReader(IEnumerable<byte> enumeration)
            {
                _enumerator = enumeration.GetEnumerator();
            }

            public byte[] GetNext(int length)
            {
                var result = new byte[length];
                for (var i = 0; i < length; i++)
                {
                    if (!_enumerator.MoveNext())
                    {
                        // hit to end => return empty array.
                        return new byte[0];
                    }
                    result[i] = _enumerator.Current;
                }
                return result;
            }

            public bool Skip(int length)
            {
                for (var i = 0; i < length; i++)
                {
                    if (!_enumerator.MoveNext())
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }

    public enum SupportedMediaTypes
    {
        Unknown,
        Bmp,
        Gif,
        Jpeg,
        Png,
        AnimatedGif,
#if WEBP_SUPPORTED
        WebP,
#endif
        Mp4,
    }
}
