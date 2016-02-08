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

        [NotNull]
        private readonly Dictionary<string, JsonValue> _dictionary;

        public override bool IsObject { get; } = true;

        public override int Count => _dictionary.Count;

        public IEnumerable<string> Keys => _dictionary.Keys;

        public IEnumerable<JsonValue> Values => _dictionary.Values;

        private JsonObject()
        {
            _dictionary = new Dictionary<string, JsonValue>();
        }

        public JsonObject(IDictionary<string, JsonValue> dictionary)
        {
            _dictionary = new Dictionary<string, JsonValue>(dictionary);
        }

        public override bool ContainsKey(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _dictionary.ContainsKey(key);
        }

        public override bool TryGetValue(string key, out JsonValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _dictionary.TryGetValue(key, out value);
        }

        public override JsonValue GetValue(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            JsonValue value;
            return _dictionary.TryGetValue(key, out value) ? value : JsonNull.Null;
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
            return _dictionary.GetHashCode();
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
