using System;

namespace Cadena.Meteor
{
    public sealed class JsonNull : JsonValue, IEquatable<JsonNull>
    {
        public static readonly JsonNull Null = new JsonNull();

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool IsNull { get; } = true;

        private JsonNull()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as JsonNull);
        }

        public bool Equals(JsonNull other)
        {
            return other != null;
        }

        public override string ToString()
        {
            return "null";
        }

        public static bool operator ==(JsonNull left, JsonNull right)
        {
            if (left == null) return right == null;
            return left.Equals(right);
        }

        public static bool operator !=(JsonNull left, JsonNull right)
        {
            return !(left == right);
        }
    }
}
