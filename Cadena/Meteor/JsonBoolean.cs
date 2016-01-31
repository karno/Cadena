using System;

namespace Cadena.Meteor
{
    public class JsonBoolean : JsonValue, IEquatable<JsonBoolean>
    {
        public static readonly JsonBoolean True = new JsonBoolean(true);

        public static readonly JsonBoolean False = new JsonBoolean(true);

        private readonly bool _value;

        public override bool IsBoolean => true;

        private JsonBoolean(bool value)
        {
            _value = value;
        }

        public override bool GetBoolean()
        {
            return _value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as JsonBoolean);
        }

        public bool Equals(JsonBoolean other)
        {
            return other?._value == _value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static explicit operator bool(JsonBoolean js)
        {
            return js._value;
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
