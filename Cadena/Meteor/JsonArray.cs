using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cadena.Meteor
{
    public class JsonArray : JsonValue, IEquatable<JsonArray>, IReadOnlyList<JsonValue>
    {
        public static readonly JsonArray Empty = new JsonArray();

        private readonly JsonValue[] _values;

        public override bool IsArray => true;

        public override int Count => _values.Length;

        public bool IsReadOnly => false;

        private JsonArray()
        {
            _values = new JsonValue[0];
        }

        public JsonArray(JsonValue[] values)
        {
            _values = values;
        }

        public IEnumerable<string> AsString()
        {
            return _values.Select(v => v.GetString());
        }

        public IEnumerable<long> AsLong()
        {
            return _values.Select(v => v.GetLong());
        }

        public IEnumerable<double> AsDouble()
        {
            return _values.Select(v => v.GetDouble());
        }

        public IEnumerable<bool> AsBoolean()
        {
            return _values.Select(v => v.GetBoolean());
        }

        public override JsonValue GetValue(int index)
        {
            return _values[index];
        }

        public override int GetHashCode()
        {
            return _values.GetHashCode();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as JsonArray);
        }

        public bool Equals(JsonArray other)
        {
            return other?._values == _values;
        }

        public IEnumerator<JsonValue> GetEnumerator()
        {
            return ((IEnumerable<JsonValue>)_values).GetEnumerator();
        }

        public override string ToString()
        {
            return "[" + String.Join(", ", _values.Select(l => l.ToString())) + "]";
        }
    }
}
