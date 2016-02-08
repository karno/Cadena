using System;
using JetBrains.Annotations;

namespace Cadena.Meteor
{
    public class JsonString : JsonValue, IEquatable<JsonString>
    {
        public string Value { get; }

        public override bool IsString { get; } = true;

        public JsonString([NotNull] string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        public override string AsString()
        {
            return Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as JsonString);
        }

        public bool Equals(JsonString other)
        {
            return other?.Value == Value;
        }

        public override string ToString()
        {
            return "\"" + Value + "\"";
        }

        public static explicit operator string(JsonString js)
        {
            return js.Value;
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
