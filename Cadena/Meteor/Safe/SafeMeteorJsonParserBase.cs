using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cadena.Meteor.Safe
{
    public abstract class SafeMeteorJsonParserBase
    {
        private const int StringBufferLength = 64;

        /// <summary>
        /// Read value.
        /// </summary>
        protected JsonValue ReadValue(ref char[] buffer, ref int index, ref int length)
        {
            // read whitespaces
            SkipWhitespaces(ref buffer, ref index, ref length);
            if (IsEndOfJson(ref buffer, ref index, ref length))
            {
                throw CreateException(buffer, index, "Json is not closed or empty.");
            }

            // switch for first letter
            switch (buffer[index])
            {
                case '[': // array
                    return ReadArray(ref buffer, ref index, ref length);

                case '{': // object
                    return ReadObject(ref buffer, ref index, ref length);

                case '"': // string
                    return ReadString(ref buffer, ref index, ref length);

                case '+':
                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9': // number
                    return ReadNumber(ref buffer, ref index, ref length);

                case 'T':
                case 't': // true
                    return ReadTrue(ref buffer, ref index, ref length);

                case 'F':
                case 'f': // false
                    return ReadFalse(ref buffer, ref index, ref length);

                case 'N':
                case 'n': // null
                    return ReadNull(ref buffer, ref index, ref length);

                default:
                    throw CreateException(buffer, index, "invalid token found.");
            }
        }

        private JsonArray ReadArray(ref char[] buffer, ref int index, ref int length)
        {
            // check first letter
            Debug.Assert(buffer[index] == '[');
            index++;

            // check array is empty
            SkipWhitespaces(ref buffer, ref index, ref length);
            if (IsEndOfJson(ref buffer, ref index, ref length))
            {
                // not completed
                throw CreateException(buffer, index, "array is not closed.");
            }
            if (buffer[index] == ']')
            {
                index++;
                return JsonArray.Empty;
            }

            // read array content
            var list = new List<JsonValue>();
            while (true)
            {
                list.Add(ReadValue(ref buffer, ref index, ref length));

                // read close bracket or comma
                SkipWhitespaces(ref buffer, ref index, ref length);

                if (IsEndOfJson(ref buffer, ref index, ref length))
                {
                    // not completed
                    throw CreateException(buffer, index, "array is not closed.");
                }

                // if length of array, next letter should be ']'.
                if (buffer[index] == ']')
                {
                    // length of array
                    index++;
                    break;
                }

                // otherwise, next letter should be ','.
                AssertAndReadNext(ref buffer, ref index, ',');
            }
            return new JsonArray(list.ToArray());
        }

        private JsonObject ReadObject(ref char[] buffer, ref int index, ref int length)
        {
            // check first letter
            Debug.Assert(buffer[index] == '{');
            index++;

            // check object is empty
            SkipWhitespaces(ref buffer, ref index, ref length);
            if (IsEndOfJson(ref buffer, ref index, ref length))
            {
                // not completed
                throw CreateException(buffer, index, "object is not closed.");
            }
            if (buffer[index] == '}')
            {
                index++;
                return JsonObject.Empty;
            }

            var dict = new Dictionary<string, JsonValue>();
            while (true)
            {
                var keyBegin = index;

                // read key
                Assert(ref buffer, ref index, ref length, '\"');
                var key = ReadString(ref buffer, ref index, ref length).AsString();

                if (dict.ContainsKey(key))
                {
                    throw CreateException(buffer, keyBegin, "duplicated key detected: " + key);
                }

                // read comma
                SkipWhitespaces(ref buffer, ref index, ref length);
                AssertAndReadNext(ref buffer, ref index, ':');

                // read value and add to dictionary
                dict.Add(key, ReadValue(ref buffer, ref index, ref length));

                // read close brace or comma
                SkipWhitespaces(ref buffer, ref index, ref length);

                if (IsEndOfJson(ref buffer, ref index, ref length))
                {
                    // not completed
                    throw CreateException(buffer, index, "object is not closed.");
                }

                // if length of array, next letter should be ']'.
                if (buffer[index] == '}')
                {
                    // length of array
                    index++;
                    break;
                }

                // otherwise, next letter should be ','.
                AssertAndReadNext(ref buffer, ref index, ',');

                // skip while next string
                SkipWhitespaces(ref buffer, ref index, ref length);
            }

            return new JsonObject(dict);
        }

        private JsonString ReadString(ref char[] buffer, ref int index, ref int length)
        {
            // check first letter
            Debug.Assert(buffer[index] == '\"');
            index++;

            // for long string
            StringBuilder builder = null;

            var strbuf = new char[StringBufferLength];
            var bp = 0;
            for (; !IsEndOfJson(ref buffer, ref index, ref length) && buffer[index] != '\"'; index++)
            {
                // check buffer
                if (bp >= StringBufferLength)
                {
                    // buffer is full
                    if (builder == null)
                    {
                        builder = new StringBuilder(StringBufferLength * 2);
                    }
                    bp = 0;
                    builder.Append(strbuf);
                }

                if (buffer[index] == '\\')
                {
                    // escaped
                    index++;
                    if (!IsEndOfJson(ref buffer, ref index, ref length))
                    {
                        switch (buffer[index])
                        {
                            case '"':
                                strbuf[bp] = '"';
                                break;

                            case '\\':
                                strbuf[bp] = '\\';
                                break;

                            case '/':
                                strbuf[bp] = '/';
                                break;

                            case 'b':
                                strbuf[bp] = '\b';
                                break;

                            case 'f':
                                strbuf[bp] = '\f';
                                break;

                            case 'n':
                                strbuf[bp] = '\n';
                                break;

                            case 'r':
                                strbuf[bp] = '\r';
                                break;

                            case 't':
                                strbuf[bp] = '\t';
                                break;

                            case 'u':
                                // hex unicode
                                var code = 0;
                                for (var i = 0; i < 4; i++)
                                {
                                    index++;
                                    if (IsEndOfJson(ref buffer, ref index, ref length))
                                    {
                                        // hitting length of char
                                        break;
                                    }
                                    code <<= 4;
                                    if (buffer[index] <= '9' && buffer[index] >= '0')
                                    {
                                        code += buffer[index] - '0';
                                    }
                                    else if (buffer[index] <= 'F' && buffer[index] >= 'A')
                                    {
                                        // code += *sp - 'A' + 10
                                        code += buffer[index] - '7';
                                    }
                                    else if (buffer[index] <= 'f' && buffer[index] >= 'a')
                                    {
                                        // code += *sp - 'a' + 10
                                        code += buffer[index] - 'W';
                                    }
                                    else
                                    {
                                        // invalid code, abort processing
                                        index--;
                                        break;
                                    }
                                }
                                // we can decode 0x0000~0xffff, so we can't exceed the Char.MaxValue
                                Debug.Assert(code <= Char.MaxValue);
                                strbuf[bp] = (char)code;
                                break;

                            default:
                                // this is not registered escape code.
                                index--;
                                strbuf[bp] = '\\';
                                break;
                        }
                    }
                    else
                    {
                        index--;
                        strbuf[bp] = '\\';
                    }
                }
                else
                {
                    // copy letter
                    strbuf[bp] = buffer[index];
                }
                bp++;
            }
            if (IsEndOfJson(ref buffer, ref index, ref length))
            {
                // length in middle of string
                throw CreateException(buffer, index, "string is not closed.");
            }
            // read " and direct to next char
            AssertAndReadNext(ref buffer, ref index, '\"');
            if (builder == null)
            {
                // builder is not used
                return new JsonString(new String(strbuf, 0, bp));
            }
            // return from builder
            builder.Append(strbuf);
            return new JsonString(builder.ToString());
        }

        private JsonNumber ReadNumber(ref char[] buffer, ref int index, ref int length)
        {
            Debug.Assert(buffer[index] == '+' || buffer[index] == '-' || (buffer[index] >= '0' && buffer[index] <= '9'));

            var isNegative = false;

            //   number = [ minus ] int [ frac ] [ exp ]
            // => [sign] int [. int] [(e|E) int]

            // read sign
            if (buffer[index] == '-' || buffer[index] == '+')
            {
                // RFC7159 says sign is only for '-', but twitter sometime returns stupid JSON.
                // So we also check '+' sign.
                isNegative = buffer[index] == '-';
                index++;
            }

            // check before reading numbers
            // only call after - or +
            // otherwise, parent don't call this method.
            AssertDigit(ref buffer, ref index, ref length, "number is required after the sign.");

            // read main int
            var longValue = ReadInteger(ref buffer, ref index, ref length);
            // read frac
            if (!IsEndOfJson(ref buffer, ref index, ref length) && buffer[index] == '.')
            {
                // this is real number, pass to real parser.
                return ReadRealNumber(isNegative, longValue, ref buffer, ref index, ref length);
            }

            // read exp
            if (!IsEndOfJson(ref buffer, ref index, ref length) && (buffer[index] == 'e' || buffer[index] == 'E'))
            {
                index++;
                // read sign
                if (!IsEndOfJson(ref buffer, ref index, ref length) && (buffer[index] == '+' || buffer[index] == '-'))
                {
                    if (buffer[index] == '-')
                    {
                        // this is negative exp, real number.
                        return ReadRealNumber(isNegative, longValue, ref buffer, ref index, ref length);
                    }
                    index++;
                }
                AssertDigit(ref buffer, ref index, ref length, "number is required after the exponent sign.");

                // read exp value
                var expValue = ReadInteger(ref buffer, ref index, ref length);
                while (expValue-- > 0)
                {
                    longValue *= 10;
                }
            }
            // buffer[index] currently indicating next char of numbers.

            // number is integer.
            if (isNegative)
            {
                longValue *= -1;
            }
            return new JsonNumber(longValue);
        }

        private JsonNumber ReadRealNumber(bool isNegative, long intpart, ref char[] buffer, ref int index, ref int length)
        {
            Debug.Assert(!IsEndOfJson(ref buffer, ref index, ref length) && (buffer[index] == '.' || buffer[index] == '-'));
            var readExp = buffer[index] == '-';
            double value = 0;

            if (buffer[index] == '.')
            {
                var fracdigit = 0;
                index++;
                AssertDigit(ref buffer, ref index, ref length, "number is required after the decimal point.");
                do
                {
                    value = value * 10 + (buffer[index] - '0');
                    index++;
                    fracdigit++;
                } while (!IsEndOfJson(ref buffer, ref index, ref length) && IsDigit(buffer, index));
                while (fracdigit-- > 0)
                {
                    value /= 10;
                }
            }

            // add intpart.
            value += intpart;
            if (isNegative)
            {
                value *= -1;
            }

            // read exp
            if (readExp || (!IsEndOfJson(ref buffer, ref index, ref length) && (buffer[index] == 'e' || buffer[index] == 'E')))
            {
                var isNegativeExp = false;
                if (!readExp)
                {
                    index++;
                }
                if (!IsEndOfJson(ref buffer, ref index, ref length) && (buffer[index] == '+' || buffer[index] == '-'))
                {
                    isNegativeExp = buffer[index] == '-';
                    index++;
                }
                AssertDigit(ref buffer, ref index, ref length, "number is required after the exponent sign.");

                // read exp value
                var expValue = ReadInteger(ref buffer, ref index, ref length);
                if (isNegativeExp)
                {
                    while (expValue-- > 0)
                    {
                        value /= 10;
                    }
                }
                else
                {
                    while (expValue-- > 0)
                    {
                        value *= 10;
                    }
                }
            }

            return new JsonNumber(value);
        }

        // read values -----------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long ReadInteger(ref char[] buffer, ref int index, ref int length)
        {
            long value = 0;
            do
            {
                value = value * 10 + (buffer[index] - '0');
                index++;
            } while (!IsEndOfJson(ref buffer, ref index, ref length) && IsDigit(buffer, index));
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonBoolean ReadTrue(ref char[] buffer, ref int index, ref int length)
        {
            Debug.Assert(buffer[index] == 't' || buffer[index] == 'T');
            index++;
            AssertAndReadNext(ref buffer, ref index, ref length, 'r', 'R');
            AssertAndReadNext(ref buffer, ref index, ref length, 'u', 'U');
            AssertAndReadNext(ref buffer, ref index, ref length, 'e', 'E');
            return JsonBoolean.True;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonBoolean ReadFalse(ref char[] buffer, ref int index, ref int length)
        {
            Debug.Assert(buffer[index] == 'f' || buffer[index] == 'F');
            index++;
            AssertAndReadNext(ref buffer, ref index, ref length, 'a', 'A');
            AssertAndReadNext(ref buffer, ref index, ref length, 'l', 'L');
            AssertAndReadNext(ref buffer, ref index, ref length, 's', 'S');
            AssertAndReadNext(ref buffer, ref index, ref length, 'e', 'E');
            return JsonBoolean.True;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonNull ReadNull(ref char[] buffer, ref int index, ref int length)
        {
            Debug.Assert(buffer[index] == 'n' || buffer[index] == 'N');
            index++;
            AssertAndReadNext(ref buffer, ref index, ref length, 'u', 'U');
            AssertAndReadNext(ref buffer, ref index, ref length, 'l', 'L');
            AssertAndReadNext(ref buffer, ref index, ref length, 'l', 'L');
            return JsonNull.Null;
        }

        // assertions ------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssertAndReadNext(ref char[] buffer, ref int index, char c)
        {
            if (buffer[index] != c)
            {
                CreateException(buffer, index, $"{c} is expected in this place, but placed char is {buffer[index]}.");
            }
            index++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssertAndReadNext(ref char[] buffer, ref int index, ref int length, char c1, char c2)
        {
            if (IsEndOfJson(ref buffer, ref index, ref length) || (buffer[index] != c1 && buffer[index] != c2))
            {
                CreateException(buffer, index, $"{c1} or {c2} is expected in this place, but placed char is {buffer[index]}.");
            }
            index++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Assert(ref char[] buffer, ref int index, ref int length, char c)
        {
            if (IsEndOfJson(ref buffer, ref index, ref length) || buffer[index] != c)
            {
                CreateException(buffer, index, $"{c} is expected in this place, but placed char is {buffer[index]}.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssertDigit(ref char[] buffer, ref int index, ref int length, string message)
        {
            if (IsEndOfJson(ref buffer, ref index, ref length) || !IsDigit(buffer, index))
            {
                CreateException(buffer, index, message);
            }
        }

        // helper functions ------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SkipWhitespaces(ref char[] buffer, ref int index, ref int length)
        {
            while (!IsEndOfJson(ref buffer, ref index, ref length) && IsWhitespace(buffer, index))
            {
                index++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsWhitespace(char[] buffer, int index)
        {
            return buffer[index] == ' ' || buffer[index] == '\t' || buffer[index] == '\r' || buffer[index] == '\n';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsDigit(char[] buffer, int index)
        {
            return buffer[index] >= '0' && buffer[index] <= '9';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsEndOfJson(ref char[] buffer, ref int index, ref int length)
        {
            return index >= length && !ReadMore(ref buffer, ref index, ref length);
        }

        protected abstract bool ReadMore(ref char[] buffer, ref int index, ref int length);

        protected abstract JsonParseException CreateException(char[] buffer, int index, string message);
    }
}