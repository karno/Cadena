using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cadena.Meteor
{
    public abstract unsafe class MeteorJsonParserBase
    {
        const int StringBufferLength = 64;

        /// <summary>
        /// Read value.
        /// </summary>
        /// <param name="ptr">character pointer</param>
        /// <param name="end">indicating end of char</param>
        /// <returns></returns>
        protected JsonValue ReadValue(ref char* ptr, ref char* end)
        {
            // read whitespaces
            SkipWhitespaces(ref ptr, ref end);
            if (IsEndOfJson(ref ptr, ref end))
            {
                throw CreateException(ptr, "Json is not closed or empty.");
            }

            // switch for first letter
            switch (*ptr)
            {
                case '[': // array
                    return ReadArray(ref ptr, ref end);

                case '{': // object
                    return ReadObject(ref ptr, ref end);

                case '"': // string
                    return ReadString(ref ptr, ref end);

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
                    return ReadNumber(ref ptr, ref end);

                case 'T':
                case 't': // true
                    return ReadTrue(ref ptr, ref end);

                case 'F':
                case 'f': // false
                    return ReadFalse(ref ptr, ref end);

                case 'N':
                case 'n': // null
                    return ReadNull(ref ptr, ref end);

                default:
                    throw CreateException(ptr, "invalid token found.");
            }
        }

        private JsonArray ReadArray(ref char* ptr, ref char* end)
        {
            // check first letter
            Debug.Assert(*ptr == '[');
            ptr++;

            // check array is empty
            SkipWhitespaces(ref ptr, ref end);
            if (IsEndOfJson(ref ptr, ref end))
            {
                // not completed
                throw CreateException(ptr, "array is not closed.");
            }
            if (*ptr == ']')
            {
                ptr++;
                return JsonArray.Empty;
            }

            // read array content
            var list = new List<JsonValue>();
            while (true)
            {
                list.Add(ReadValue(ref ptr, ref end));

                // read close bracket or comma
                SkipWhitespaces(ref ptr, ref end);

                if (IsEndOfJson(ref ptr, ref end))
                {
                    // not completed
                    throw CreateException(ptr, "array is not closed.");
                }

                // if end of array, next letter should be ']'.
                if (*ptr == ']')
                {
                    // end of array
                    ptr++;
                    break;
                }

                // otherwise, next letter should be ','.
                AssertAndReadNext(ref ptr, ',');
            }
            return new JsonArray(list.ToArray());
        }

        private JsonObject ReadObject(ref char* ptr, ref char* end)
        {
            // check first letter
            Debug.Assert(*ptr == '{');
            ptr++;

            // check object is empty
            SkipWhitespaces(ref ptr, ref end);
            if (IsEndOfJson(ref ptr, ref end))
            {
                // not completed
                throw CreateException(ptr, "object is not closed.");
            }
            if (*ptr == '}')
            {
                ptr++;
                return JsonObject.Empty;
            }

            var dict = new Dictionary<string, JsonValue>();
            while (true)
            {
                var keyBegin = ptr;

                // read key
                Assert(ref ptr, ref end, '\"');
                var key = ReadString(ref ptr, ref end).GetString();

                if (dict.ContainsKey(key))
                {
                    throw CreateException(keyBegin, "duplicated key detected: " + key);
                }

                // read comma
                SkipWhitespaces(ref ptr, ref end);
                AssertAndReadNext(ref ptr, ':');

                // read value and add to dictionary
                dict.Add(key, ReadValue(ref ptr, ref end));

                // read close brace or comma
                SkipWhitespaces(ref ptr, ref end);

                if (IsEndOfJson(ref ptr, ref end))
                {
                    // not completed
                    throw CreateException(ptr, "object is not closed.");
                }

                // if end of array, next letter should be ']'.
                if (*ptr == '}')
                {
                    // end of array
                    ptr++;
                    break;
                }

                // otherwise, next letter should be ','.
                AssertAndReadNext(ref ptr, ',');

                // skip while next string
                SkipWhitespaces(ref ptr, ref end);
            }

            return new JsonObject(dict);
        }

        private JsonString ReadString(ref char* ptr, ref char* end)
        {
            // check first letter
            Debug.Assert(*ptr == '\"');
            ptr++;

            // for long string
            StringBuilder builder = null;

            var buffer = new char[StringBufferLength];
            fixed (char* bufptr = buffer)
            {
                var bp = bufptr;
                var bend = bufptr + StringBufferLength - 1;
                for (; !IsEndOfJson(ref ptr, ref end) && *ptr != '\"'; ptr++)
                {
                    // check buffer
                    if (bp > bend)
                    {
                        // buffer is full
                        if (builder == null)
                        {
                            builder = new StringBuilder(StringBufferLength * 2);
                        }
                        bp = bufptr;
                        builder.Append(bp, StringBufferLength);
                    }

                    if (*ptr == '\\')
                    {
                        // escaped
                        ptr++;
                        if (!IsEndOfJson(ref ptr, ref end))
                        {
                            switch (*ptr)
                            {
                                case '"':
                                    *bp = '"';
                                    break;
                                case '\\':
                                    *bp = '\\';
                                    break;
                                case '/':
                                    *bp = '/';
                                    break;
                                case 'b':
                                    *bp = '\b';
                                    break;
                                case 'f':
                                    *bp = '\f';
                                    break;
                                case 'n':
                                    *bp = '\n';
                                    break;
                                case 'r':
                                    *bp = '\r';
                                    break;
                                case 't':
                                    *bp = '\t';
                                    break;
                                case 'u':
                                    // hex unicode
                                    var code = 0;
                                    for (var i = 0; i < 4; i++)
                                    {
                                        ptr++;
                                        if (IsEndOfJson(ref ptr, ref end))
                                        {
                                            // hitting end of char
                                            break;
                                        }
                                        code <<= 4;
                                        if (*ptr <= '9' && *ptr >= '0')
                                        {
                                            code += *ptr - '0';
                                        }
                                        else if (*ptr <= 'F' && *ptr >= 'A')
                                        {
                                            // code += *sp - 'A' + 10
                                            code += *ptr - '7';
                                        }
                                        else if (*ptr <= 'f' && *ptr >= 'a')
                                        {
                                            // code += *sp - 'a' + 10
                                            code += *ptr - 'W';
                                        }
                                        else
                                        {
                                            // invalid code, abort processing
                                            ptr--;
                                            break;
                                        }
                                    }
                                    // we can decode 0x0000~0xffff, so we can't exceed the Char.MaxValue
                                    Debug.Assert(code <= Char.MaxValue);
                                    *bp = (char)code;
                                    break;

                                default:
                                    // this is not registered escape code.
                                    ptr--;
                                    *bp = '\\';
                                    break;
                            }
                        }
                        else
                        {
                            ptr--;
                            *bp = '\\';
                        }
                    }
                    else
                    {
                        // copy letter
                        *bp = *ptr;
                    }
                    bp++;
                }
                if (IsEndOfJson(ref ptr, ref end))
                {
                    // end in middle of string
                    throw CreateException(ptr, "string is not closed.");
                }
                // read " and direct to next char
                AssertAndReadNext(ref ptr, '\"');
                if (builder == null)
                {
                    // builder is not used
                    return new JsonString(new String(bufptr, 0, (int)(bp - bufptr)));
                }
                // return from builder
                builder.Append(bufptr, (int)(bp - bufptr));
            }
            return new JsonString(builder.ToString());
        }

        private JsonNumber ReadNumber(ref char* ptr, ref char* end)
        {
            Debug.Assert(*ptr == '+' || *ptr == '-' || (*ptr >= '0' && *ptr <= '9'));

            var isNegative = false;

            //   number = [ minus ] int [ frac ] [ exp ]
            // => [sign] int [. int] [(e|E) int]

            // read sign
            if (*ptr == '-' || *ptr == '+')
            {
                // RFC7159 says sign is only for '-', but twitter sometime returns stupid JSON. 
                // So we also check '+' sign.
                isNegative = *ptr == '-';
                ptr++;
            }

            // check before reading integer 
            // only call after - or +
            // otherwise, parent don't call this method.
            AssertDigit(ref ptr, ref end, "number is required after the sign.");

            // read main integer
            var longValue = ReadInteger(ref ptr, ref end);

            // read frac
            if (!IsEndOfJson(ref ptr, ref end) && *ptr == '.')
            {
                // this is real number, pass to real parser.
                return ReadRealNumber(isNegative, longValue, ref ptr, ref end);
            }

            // read exp
            if (!IsEndOfJson(ref ptr, ref end) && (*ptr == 'e' || *ptr == 'E'))
            {
                ptr++;
                // read sign
                if (!IsEndOfJson(ref ptr, ref end) && (*ptr == '+' || *ptr == '-'))
                {
                    if (*ptr == '-')
                    {
                        // this is negative exp, real number.
                        return ReadRealNumber(isNegative, longValue, ref ptr, ref end);
                    }
                    ptr++;
                }

                // check before reading exp value
                AssertDigit(ref ptr, ref end, "number is required after the exponent sign.");

                // read exp value
                var expValue = ReadInteger(ref ptr, ref end);
                while (expValue-- > 0)
                {
                    longValue *= 10;
                }
            }
            // *ptr currently indicating next char of numbers.

            // number is integer.
            if (isNegative)
            {
                longValue *= -1;
            }
            return new JsonNumber(longValue);
        }

        private JsonNumber ReadRealNumber(bool isNegative, long intpart, ref char* ptr, ref char* end)
        {
            Debug.Assert(!IsEndOfJson(ref ptr, ref end) && (*ptr == '.' || *ptr == '-'));
            var readExp = *ptr == '-';
            double value = 0;

            if (*ptr == '.')
            {
                var fracdigit = 0;
                ptr++;
                AssertDigit(ref ptr, ref end, "number is required after the decimal point.");
                do
                {
                    value = value * 10 + (*ptr - '0');
                    ptr++;
                    fracdigit++;
                } while (!IsEndOfJson(ref ptr, ref end) && IsDigit(ptr));
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
            if (readExp || (!IsEndOfJson(ref ptr, ref end) && (*ptr == 'e' || *ptr == 'E')))
            {
                var isNegativeExp = false;
                if (!readExp)
                {
                    ptr++;
                }
                if (!IsEndOfJson(ref ptr, ref end) && (*ptr == '+' || *ptr == '-'))
                {
                    isNegativeExp = *ptr == '-';
                    ptr++;
                }
                AssertDigit(ref ptr, ref end, "number is required after the exponent sign.");

                // read exp value
                var expValue = ReadInteger(ref ptr, ref end);
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
        private long ReadInteger(ref char* ptr, ref char* end)
        {
            long value = 0;
            do
            {
                value = value * 10 + (*ptr - '0');
                ptr++;
            } while (!IsEndOfJson(ref ptr, ref end) && IsDigit(ptr));
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonBoolean ReadTrue(ref char* ptr, ref char* end)
        {
            Debug.Assert(*ptr == 't' || *ptr == 'T');
            ptr++;
            AssertAndReadNext(ref ptr, ref end, 'r', 'R');
            AssertAndReadNext(ref ptr, ref end, 'u', 'U');
            AssertAndReadNext(ref ptr, ref end, 'e', 'E');
            return JsonBoolean.True;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonBoolean ReadFalse(ref char* ptr, ref char* end)
        {
            Debug.Assert(*ptr == 'f' || *ptr == 'F');
            ptr++;
            AssertAndReadNext(ref ptr, ref end, 'a', 'A');
            AssertAndReadNext(ref ptr, ref end, 'l', 'L');
            AssertAndReadNext(ref ptr, ref end, 's', 'S');
            AssertAndReadNext(ref ptr, ref end, 'e', 'E');
            return JsonBoolean.True;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonNull ReadNull(ref char* ptr, ref char* end)
        {
            Debug.Assert(*ptr == 'n' || *ptr == 'N');
            ptr++;
            AssertAndReadNext(ref ptr, ref end, 'u', 'U');
            AssertAndReadNext(ref ptr, ref end, 'l', 'L');
            AssertAndReadNext(ref ptr, ref end, 'l', 'L');
            return JsonNull.Null;
        }

        // assertions ------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssertAndReadNext(ref char* ptr, char c)
        {
            if (*ptr != c)
            {
                CreateException(ptr, $"{c} is expected in this place, but placed char is {*ptr}.");
            }
            ptr++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssertAndReadNext(ref char* ptr, ref char* end, char c1, char c2)
        {
            if (IsEndOfJson(ref ptr, ref end) || (*ptr != c1 && *ptr != c2))
            {
                CreateException(ptr, $"{c1} or {c2} is expected in this place, but placed char is {*ptr}.");
            }
            ptr++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Assert(ref char* ptr, ref char* end, char c)
        {
            if (IsEndOfJson(ref ptr, ref end) || *ptr != c)
            {
                CreateException(ptr, $"{c} is expected in this place, but placed char is {*ptr}.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssertDigit(ref char* ptr, ref char* end, string message)
        {
            if (IsEndOfJson(ref ptr, ref end) || !IsDigit(ptr))
            {
                throw CreateException(ptr, message);
            }
        }

        // helper functions ------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SkipWhitespaces(ref char* ptr, ref char* end)
        {
            while (!IsEndOfJson(ref ptr, ref end) && IsWhitespace(ptr))
            {
                ptr++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsWhitespace(char* c)
        {
            return *c == ' ' || *c == '\t' || *c == '\r' || *c == '\n';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsDigit(char* c)
        {
            return *c >= '0' && *c <= '9';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsEndOfJson(ref char* ptr, ref char* end)
        {
            return ptr > end && !ReadMore(ref ptr, ref end);
        }

        protected abstract bool ReadMore(ref char* ptr, ref char* end);

        protected abstract JsonParseException CreateException(char* ptr, string message);

    }
}
