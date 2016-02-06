namespace Cadena.Meteor
{
    public static class MeteorJson
    {
        public static JsonValue Parse(string text)
        {
            return new JsonStringParser(text).Parse();
        }

        private sealed class JsonStringParser : MeteorJsonParserBase
        {
            private readonly string _json;
            private unsafe char* _begin = null;

            public JsonStringParser(string json)
            {
                _json = json;
            }

            public unsafe JsonValue Parse()
            {
                try
                {
                    fixed (char* jsonptr = _json)
                    {
                        var ptr = _begin = jsonptr;
                        var end = jsonptr + _json.Length - 1;
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
                    _begin = null;
                }
            }

            protected override unsafe bool ReadMore(ref char* ptr, ref char* end)
            {
                return false;
            }

            protected override unsafe JsonParseException CreateException(char* ptr, string message)
            {
                var index = ptr - _begin;
                if (index > _json.Length)
                {
                    index = _json.Length;
                }
                throw new JsonParseException(message, _json, index);
            }
        }
    }
}
