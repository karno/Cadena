using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Cadena.Meteor
{
    [DataContract]
    public class JsonParseException : Exception
    {
        [DataMember]
        private readonly string _json;

        [DataMember]
        private readonly long _index;

        public JsonParseException(string json, long index)
        {
            _json = json;
            _index = index;
        }

        public JsonParseException(string message, string json, long index) : base(message)
        {
            _json = json;
            _index = index;
        }

        public JsonParseException(string message, string json, long index, Exception inner) : base(message, inner)
        {
            _json = json;
            _index = index;
        }

        public override string ToString()
        {
            var cursor = 5;
            if (_index < 5)
            {
                cursor = 0;
            }
            // show before 5 chars, after 15 chars
            var begin = (int)(_index - cursor);
            var substring = _json.Length > begin ? _json.Substring(begin) : _json;
            if (substring.Length > 20)
            {
                substring = substring.Substring(0, 20);
            }
            var spaces = new string(Enumerable.Range(0, cursor).Select(_ => ' ').ToArray());
            var exinfo = InnerException == null ? String.Empty : Environment.NewLine + "inner ex: " + InnerException;
            return "Failed to decode JSON." + Environment.NewLine +
                   "INDEX " + _index + ", " + Message + Environment.NewLine +
                   "trailing character: " + substring + Environment.NewLine +
                   "                    " + spaces + "^" + Environment.NewLine +
                   "full text: " + _json + exinfo;
        }
    }
}