using System;
using JetBrains.Annotations;

namespace Cadena.Meteor
{
    public abstract class JsonValue : IEquatable<JsonValue>
    {
        public virtual bool IsObject { get; } = false;

        public virtual bool IsString { get; } = false;

        public virtual bool IsNumber { get; } = false;

        public virtual bool IsArray { get; } = false;

        public virtual bool IsBoolean { get; } = false;

        public virtual bool IsNull { get; } = false;

        public virtual int Count
        {
            get { throw new NotSupportedException(); }
        }

        [NotNull]
        public JsonValue this[int index]
        {
            get { return GetValue(index); }
        }

        [NotNull]
        public JsonValue this[string key]
        {
            get { return GetValue(key); }
        }

        public virtual bool ContainsKey([NotNull] string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return false;
        }

        public virtual bool TryGetValue([NotNull] string key, out JsonValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            value = null;
            return false;
        }

        [NotNull]
        public virtual JsonValue GetValue(int index)
        {
            if (index <= 0) throw new ArgumentOutOfRangeException(nameof(index));
            return JsonNull.Null;
        }

        [NotNull]
        public virtual JsonValue GetValue([NotNull] string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return JsonNull.Null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as JsonValue);
        }

        public override int GetHashCode()
        {
            // dummy method, child class should implement this.
            return 0;
        }

        public abstract bool Equals(JsonValue other);

        [CanBeNull]
        public virtual string AsString()
        {
            return null;
        }

        public virtual long AsLong()
        {
            return default(long);
        }

        public virtual double AsDouble()
        {
            return default(double);
        }

        public virtual bool AsBoolean()
        {
            return default(bool);
        }

        [CanBeNull]
        public JsonArray AsArray()
        {
            return this as JsonArray;
        }

        [CanBeNull]
        public JsonObject AsObject()
        {
            return this as JsonObject;
        }

        public int AsInteger()
        {
            return (int)AsLong();
        }

        public long? AsLongOrNull()
        {
            return IsNumber ? (long?)AsLong() : null;
        }

        public int? AsIntegerOrNull()
        {
            return IsNumber ? (int?)AsInteger() : null;
        }


        public double? AsDoubleOrNull()
        {
            return IsNumber ? (double?)AsDouble() : null;
        }

        public bool? AsBooleanOrNull()
        {
            return IsBoolean ? (bool?)AsBoolean() : null;
        }


        #region overriding comparators

        public static bool operator ==(JsonValue left, object right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (ReferenceEquals(left, null) || ReferenceEquals(left, JsonNull.Null))
            {
                return ReferenceEquals(right, JsonNull.Null) || right == null;
            }
            return left.Equals(right);
        }

        public static bool operator !=(JsonValue left, object right)
        {
            return !(left == right);
        }

        #endregion
    }
}
