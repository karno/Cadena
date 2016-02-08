using System;

namespace Cadena.Meteor
{
    public class JsonNumber : JsonValue, IEquatable<JsonNumber>
    {
        public JsonNumber(long longValue)
        {
            IsInteger = true;
            LongValue = longValue;
        }

        public JsonNumber(double doubleValue)
        {
            IsInteger = false;
            DoubleValue = doubleValue;
        }

        public bool IsInteger { get; }

        public long LongValue { get; }

        public double DoubleValue { get; }

        public override bool IsNumber { get; } = true;

        public override long AsLong()
        {
            return IsInteger ? LongValue : (long)DoubleValue;
        }

        public override double AsDouble()
        {
            return IsInteger ? LongValue : DoubleValue;
        }

        public override int GetHashCode()
        {
            return IsInteger.GetHashCode() ^ LongValue.GetHashCode() ^ DoubleValue.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as JsonNumber);
        }

        public bool Equals(JsonNumber other)
        {
            if (other == null) return false;
            return other.IsInteger
                ? other.LongValue == LongValue
                : Math.Abs(other.DoubleValue - DoubleValue) < Double.Epsilon;
        }

        public static explicit operator long(JsonNumber number)
        {
            return number.AsLong();
        }

        public static explicit operator double(JsonNumber number)
        {
            return number.AsDouble();
        }

        public static bool operator ==(JsonNumber left, JsonNumber right)
        {
            if (left == null) return right == null;
            return left.Equals(right);
        }

        public static bool operator !=(JsonNumber left, JsonNumber right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return IsInteger ? LongValue.ToString() : DoubleValue.ToString();
        }
    }
}
