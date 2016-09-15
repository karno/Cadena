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

        public override bool Equals(object other)
        {
            return ReferenceEquals(other, Null) || other == null;
        }

        public override bool Equals(JsonValue other)
        {
            return Equals((object)other);
        }

        public bool Equals(JsonNull other)
        {
            return Equals((object)other);
        }

        public override string ToString()
        {
            return "null";
        }
    }
}
