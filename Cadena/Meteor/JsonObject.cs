using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Cadena.Meteor
{
    public class JsonObject : JsonValue, IEquatable<JsonObject>, IReadOnlyDictionary<string, JsonValue>
    {
        public static readonly JsonObject Empty = new JsonObject();

        private readonly Dictionary<string, JsonValue> _dictionary;

        public override bool IsObject => true;

        public override int Count => _dictionary.Count;

        public ICollection<string> Keys => _dictionary.Keys;

        IEnumerable<string> IReadOnlyDictionary<string, JsonValue>.Keys => Keys;

        public ICollection<JsonValue> Values => _dictionary.Values;

        IEnumerable<JsonValue> IReadOnlyDictionary<string, JsonValue>.Values => Values;

        private JsonObject()
        {
            _dictionary = new Dictionary<string, JsonValue>();
        }

        public JsonObject(IDictionary<string, JsonValue> dictionary)
        {
            _dictionary = new Dictionary<string, JsonValue>(dictionary);
        }

        public override bool ContainsKey([NotNull] string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _dictionary.ContainsKey(key);
        }

        public override bool TryGetValue(string key, out JsonValue value)
        {
            return base.TryGetValue(key, out value);
        }

        public override JsonValue GetValue(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _dictionary[key];
        }

        public IEnumerator<KeyValuePair<string, JsonValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as JsonObject);
        }

        public override int GetHashCode()
        {
            return _dictionary?.GetHashCode() ?? 0;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(JsonObject other)
        {
            return other?._dictionary == _dictionary;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var values in _dictionary)
            {
                builder.AppendLine(new JsonString(values.Key) + ": " + values.Value);
            }
            return "{" + Environment.NewLine + builder + "}" + Environment.NewLine;
        }
    }
}
