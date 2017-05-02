using System;
using System.IO;
using System.Text;

namespace Cadena.Meteor.Safe
{
    public sealed class SafeJsonStreamParser : SafeMeteorJsonParserBase, IDisposable
    {
        private const int BufferSize = 2048;

        private readonly StreamReader _reader;

        public SafeJsonStreamParser(Stream stream)
            : this(stream, Encoding.UTF8)
        {
        }

        public SafeJsonStreamParser(Stream stream, Encoding encoding)
            : this(new StreamReader(stream, encoding))
        {
        }

        public SafeJsonStreamParser(StreamReader reader)
        {
            _reader = reader;
        }

        public JsonValue Parse()
        {
            var buffer = new char[BufferSize];
            var index = 0;
            var length = 0;
            ReadMore(ref buffer, ref index, ref length);
            // read value
            var value = ReadValue(ref buffer, ref index, ref length);
            // check after object
            SkipWhitespaces(ref buffer, ref index, ref length);
            if (index < length)
            {
                throw CreateException(buffer, index,
                    "invalid character is existed after the valid object: " + buffer[index]);
            }
            // return result
            return value;
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        protected override bool ReadMore(ref char[] buffer, ref int index, ref int length)
        {
            index = 0;
            length = _reader.Read(buffer, index, buffer.Length);
            return length > 0;
        }

        protected override JsonParseException CreateException(char[] buffer, int index, string message)
        {
            if (index > buffer.Length)
            {
                index = buffer.Length;
            }
            throw new JsonParseException(message, new String(buffer), index);
        }
    }
}