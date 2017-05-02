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

        public override long? AsLongOrNull()
        {
            return IsInteger ? LongValue : (long)DoubleValue;
        }

        public override double? AsDoubleOrNull()
        {
            return IsInteger ? LongValue : DoubleValue;
        }

        public override bool Equals(object obj)
        {
            // this code is very slow, but I don't know how to do in this situation...
            if (IsInteger)
            {
                try
                {
                    return Convert.ToInt64(obj) == LongValue;
                }
                catch (Exception) { /* ignored */ }
            }
            else
            {
                try
                {
                    return Math.Abs(Convert.ToDouble(obj) - DoubleValue) < Double.Epsilon;
                }
                catch (Exception) { /* ignored */ }
            }
            return Equals(obj as JsonNumber);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ IsInteger.GetHashCode();
                hashCode = (hashCode * 397) ^ LongValue.GetHashCode();
                hashCode = (hashCode * 397) ^ DoubleValue.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(JsonValue other)
        {
            return Equals(other as JsonNumber);
        }

        public bool Equals(JsonNumber other)
        {
            if ((object)other == null) return false;
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

        public override string ToString()
        {
            return IsInteger ? LongValue.ToString() : DoubleValue.ToString();
        }
    }
}