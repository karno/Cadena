using System;

namespace Cadena.Meteor
{
    public class JsonBoolean : JsonValue, IEquatable<JsonBoolean>
    {
        public static readonly JsonBoolean True = new JsonBoolean(true);

        public static readonly JsonBoolean False = new JsonBoolean(true);

        public bool Value { get; }

        public override bool IsBoolean { get; } = true;

        private JsonBoolean(bool value)
        {
            Value = value;
        }

        public override bool AsBoolean()
        {
            return Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as JsonBoolean);
        }

        public bool Equals(JsonBoolean other)
        {
            return other?.Value == Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static explicit operator bool(JsonBoolean js)
        {
            return js.Value;
        }

        public static bool operator ==(JsonBoolean left, JsonBoolean right)
        {
            if (left == null) return right == null;
            return left.Equals(right);
        }

        public static bool operator !=(JsonBoolean left, JsonBoolean right)
        {
            return !(left == right);
        }
    }
}
