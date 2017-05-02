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
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override string AsStringOrNull()
        {
            return Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is string str) // str should be not null
            {
                return str == Value;
            }
            return Equals(obj as JsonString);
        }

        public override bool Equals(JsonValue other)
        {
            return Equals(other as JsonString);
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
            if ((object)left == null) return (object)right == null;
            return left.Equals(right);
        }

        public static bool operator !=(JsonString left, JsonString right)
        {
            return !(left == right);
        }
    }
}