using System;
using System.IO;
using System.Text;
using Cadena.Meteor._Internals;

namespace Cadena.Meteor
{
    public sealed class JsonStreamReader : MeteorJsonParserBase, IDisposable
    {
        const int BufferSize = 1024;

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

    public sealed class JsonSafeStreamReader : MeteorSafeJsonParserBase, IDisposable
    {
        const int BufferSize = 1024;

        private readonly StreamReader _reader;

        public JsonSafeStreamReader(Stream stream)
            : this(stream, Encoding.UTF8)
        {
        }

        public JsonSafeStreamReader(Stream stream, Encoding encoding)
            : this(new StreamReader(stream, encoding))
        {

        }

        public JsonSafeStreamReader(StreamReader reader)
        {
            _reader = reader;
        }

        public unsafe JsonValue Parse()
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
