using System;
using JetBrains.Annotations;

namespace Cadena.Meteor
{
    public class JsonString : JsonValue, IEquatable<JsonString>
    {
        private readonly string _value;

        public override bool IsString => true;

        public JsonString([NotNull] string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            _value = value;
        }

        public override string GetString()
        {
            return _value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as JsonString);
        }

        public bool Equals(JsonString other)
        {
            return other?._value == _value;
        }

        public override string ToString()
        {
            return "\"" + _value + "\"";
        }

        public static explicit operator string(JsonString js)
        {
            return js._value;
        }

        public static bool operator ==(JsonString left, JsonString right)
        {
            if (left == null) return right == null;
            return left.Equals(right);
        }

        public static bool operator !=(JsonString left, JsonString right)
        {
            return !(left == right);
        }
    }
}
