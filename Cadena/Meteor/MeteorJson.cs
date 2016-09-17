namespace Cadena.Meteor
{
    public static class MeteorJson
    {
        public static JsonValue Parse(string text)
        {
            return new JsonStringParser().Parse(text);
        }
    }

    public sealed class JsonStringParser : MeteorJsonParserBase
    {
        private string _json;
        private unsafe char* _begin = null;

        public unsafe JsonValue Parse(string json)
        {
            _json = json;
            try
            {
                fixed (char* jsonptr = json)
                {
                    var ptr = _begin = jsonptr;
                    var end = jsonptr + json.Length - 1;
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
                _json = null;
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
