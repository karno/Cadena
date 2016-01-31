using System;

namespace Cadena.Meteor
{
    public class JsonNumber : JsonValue, IEquatable<JsonNumber>
    {
        private readonly bool _isInteger;
        private readonly long _longValue;
        private readonly double _doubleValue;

        public JsonNumber(long longValue)
        {
            _isInteger = true;
            _longValue = longValue;
        }

        public JsonNumber(double doubleValue)
        {
            _isInteger = false;
            _doubleValue = doubleValue;
        }

        public override bool IsNumber => true;

        public override long GetLong()
        {
            return _isInteger ? _longValue : (long)_doubleValue;
        }

        public override double GetDouble()
        {
            return _isInteger ? _longValue : _doubleValue;
        }

        public override int GetHashCode()
        {
            return _isInteger.GetHashCode() ^ _longValue.GetHashCode() ^ _doubleValue.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as JsonNumber);
        }

        public bool Equals(JsonNumber other)
        {
            if (other == null) return false;
            return other._isInteger
                ? other._longValue == _longValue
                : other._doubleValue == _doubleValue;
        }

        public static explicit operator long(JsonNumber number)
        {
            return number.GetLong();
        }

        public static explicit operator double(JsonNumber number)
        {
            return number.GetDouble();
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
            return _isInteger ? _longValue.ToString() : _doubleValue.ToString();
        }
    }
}
