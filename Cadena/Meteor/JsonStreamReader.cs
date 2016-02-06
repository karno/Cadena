using System;
using System.IO;
using System.Text;

namespace Cadena.Meteor
{
    public sealed class JsonStreamReader : MeteorJsonParserBase, IDisposable
    {
        const int BufferSize = 2048;

        private readonly StreamReader _reader;

        private char[] _buffer;
        private unsafe char* _begin = null;

        public JsonStreamReader(Stream stream)
            : this(stream, Encoding.UTF8)
        {
        }

        public JsonStreamReader(Stream stream, Encoding encoding)
            : this(new StreamReader(stream, encoding))
        {

        }

        public JsonStreamReader(StreamReader reader)
        {
            _reader = reader;
        }

        public unsafe JsonValue Parse()
        {
            try
            {
                _buffer = new char[BufferSize];
                fixed (char* jsonptr = _buffer)
                {
                    var ptr = _begin = jsonptr;
                    var end = jsonptr;
                    ReadMore(ref ptr, ref end);
                    // read value
                    var value = ReadValue(ref ptr, ref end);
                    // check after object
                    SkipWhitespaces(ref ptr, ref end);
                    if (ptr <= end)
                    {
                        throw CreateException(ptr, "invalid character is existed after the valid object: " + *ptr);
                    }
                    // return result
                    return value;
                }
            }
            finally
            {
                _buffer = null;
                _begin = null;
            }
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        protected override unsafe bool ReadMore(ref char* ptr, ref char* end)
        {
            var read = _reader.Read(_buffer, 0, BufferSize);
            if (read == 0)
            {
                return false;
            }
            ptr = _begin;
            end = _begin + read - 1;
            return true;
        }

        protected override unsafe JsonParseException CreateException(char* ptr, string message)
        {
            var index = ptr - _begin;
            if (index > _buffer.Length)
            {
                index = _buffer.Length;
            }
            throw new JsonParseException(message, new String(_buffer), index);
        }
    }
}
