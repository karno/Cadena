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

        public override bool IsArray { get; } = true;

        public override int Count => _values.Length;

        public bool IsReadOnly { get; } = false;

        private JsonArray()
        {
            _values = new JsonValue[0];
        }

        public JsonArray(JsonValue[] values)
        {
            _values = values;
        }

        public string[] AsStringArray()
        {
            return _values.Select(v => v.AsString()).ToArray();
        }

        public long[] AsLongArray()
        {
            return _values.Select(v => v.AsLong()).ToArray();
        }

        public double[] AsDoubleArray()
        {
            return _values.Select(v => v.AsDouble()).ToArray();
        }

        public bool[] AsBooleanArray()
        {
            return _values.Select(v => v.AsBoolean()).ToArray();
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

        public override bool Equals(JsonValue other)
        {
            return Equals(other as JsonArray);
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
