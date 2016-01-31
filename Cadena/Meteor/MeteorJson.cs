using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cadena.Meteor
{
    /// <summary>
    /// Meruru's lethal weapon: Infinity Meteor (Cost Bonus, Sonic Throw, ...)
    /// </summary>
    public static class MeteorJson
    {
        /// <summary>
        /// Parse JSON string and generate object graph.
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <returns>generated tree</returns>
        public static unsafe JsonValue Parse(string json)
        {
            fixed (char* jsonptr = json)
            {
                var end = jsonptr + json.Length - 1;
                var ptr = jsonptr;
                var thrower = new JsonExceptionFactory(jsonptr, json);
                // read value
                var value = ReadValue(ref ptr, end, thrower);
                // check after object
                SkipWhitespaces(ref ptr, end);
                if (ptr <= end)
                {
                    throw thrower.CreateException(ptr, "invalid character is existed after the valid object: " + *ptr);
                }
                // return result
                return value;
            }
        }

        private static unsafe JsonValue ReadValue(ref char* ptr, char* end, JsonExceptionFactory factory)
        {
            // read whitespaces
            SkipWhitespaces(ref ptr, end);
            if (ptr > end)
            {
                throw factory.CreateException(ptr, "Json is not closed or empty.");
            }

            // switch for first letter
            switch (*ptr)
            {
                case '[': // array
                    return ReadArray(ref ptr, end, factory);

                case '{': // object
                    return ReadObject(ref ptr, end, factory);

                case '"': // string
                    return ReadString(ref ptr, end, factory);

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
                    return ReadNumber(ref ptr, end, factory);

                case 'T':
                case 't': // true
                    return ReadTrue(ref ptr, end, factory);

                case 'F':
                case 'f': // false
                    return ReadFalse(ref ptr, end, factory);

                case 'N':
                case 'n': // null
                    return ReadNull(ref ptr, end, factory);

                default:
                    throw factory.CreateException(ptr, "invalid token found.");
            }
        }

        private static unsafe JsonArray ReadArray(ref char* ptr, char* end, JsonExceptionFactory factory)
        {
            // check first letter
            Debug.Assert(*ptr == '[');
            ptr++;

            // check array is empty
            SkipWhitespaces(ref ptr, end);
            if (*ptr == ']')
            {
                ptr++;
                return JsonArray.Empty;
            }

            // read array content
            var list = new List<JsonValue>();
            while (true)
            {
                list.Add(ReadValue(ref ptr, end, factory));

                // read close bracket or comma
                SkipWhitespaces(ref ptr, end);

                if (ptr > end)
                {
                    // not completed
                    throw factory.CreateException(ptr, "array is not closed.");
                }

                // if end of array, next letter should be ']'.
                if (*ptr == ']')
                {
                    // end of array
                    ptr++;
                    break;
                }

                // otherwise, next letter should be ','.
                factory.Assert(ptr, ',');
                ptr++;
            }
            return new JsonArray(list.ToArray());
        }

        private static unsafe JsonObject ReadObject(ref char* ptr, char* end, JsonExceptionFactory factory)
        {
            // check first letter
            Debug.Assert(*ptr == '{');
            ptr++;

            // check object is empty
            SkipWhitespaces(ref ptr, end);
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
                factory.Assert(ptr, '\"');
                var key = ReadString(ref ptr, end, factory).GetString();

                if (dict.ContainsKey(key))
                {
                    throw factory.CreateException(keyBegin, "duplicated key detected: " + key);
                }

                // read comma
                SkipWhitespaces(ref ptr, end);
                factory.Assert(ptr, ':');
                ptr++;

                // read value and add to dictionary
                dict.Add(key, ReadValue(ref ptr, end, factory));

                // read close brace or comma
                SkipWhitespaces(ref ptr, end);

                if (ptr > end)
                {
                    // not completed
                    throw factory.CreateException(ptr, "object is not closed.");
                }

                // if end of array, next letter should be ']'.
                if (*ptr == '}')
                {
                    // end of array
                    ptr++;
                    break;
                }

                // otherwise, next letter should be ','.
                factory.Assert(ptr, ',');
                ptr++;

                // skip while next string
                SkipWhitespaces(ref ptr, end);
            }

            return new JsonObject(dict);
        }

        private static unsafe JsonString ReadString(ref char* ptr, char* end, JsonExceptionFactory factory)
        {
            // check first letter
            Debug.Assert(*ptr == '\"');
            ptr++;

            // for long string
            StringBuilder builder = null;

            var buffer = new char[1024];
            fixed (char* bufptr = buffer)
            {
                var bp = bufptr;
                var bend = bufptr + buffer.Length - 1;
                for (; ptr <= end && *ptr != '\"'; ptr++)
                {
                    // check buffer
                    if (bp > bend)
                    {
                        // buffer is full
                        if (builder == null)
                        {
                            builder = new StringBuilder(2048);
                        }
                        bp = bufptr;
                        builder.Append(bp, 1024);
                    }

                    if (*ptr == '\\')
                    {
                        // escaped
                        ptr++;
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
                                        ptr -= i;
                                        goto default;
                                    }
                                }
                                if (code > Char.MaxValue)
                                {
                                    // out of range
                                    ptr -= 4;
                                    goto default;
                                }
                                *bp = (char)code;
                                break;
                            default:
                                // this is not registered escape code.
                                ptr--;
                                *bp = *ptr;
                                break;
                        }
                    }
                    else
                    {
                        // copy letter
                        *bp = *ptr;
                    }
                    bp++;
                }
                if (ptr > end)
                {
                    // end in middle of string
                    throw factory.CreateException(ptr, "string is not closed.");
                }
                factory.Assert(ptr, '\"');
                // specify next char
                ptr++;
                if (builder == null)
                {
                    // builder is not used
                    return new JsonString(new String(bufptr, 0, (int)(bp - bufptr)));
                }
                // return from builder
                builder.Append(bufptr, (int)(bp - bufptr));
                return new JsonString(builder.ToString());
            }
        }

        private static unsafe JsonNumber ReadNumber(ref char* ptr, char* end, JsonExceptionFactory factory)
        {
            Debug.Assert(*ptr == '+' || *ptr == '-' || (*ptr >= '0' && *ptr <= '9'));

            var begin = ptr;
            var isFrac = false;
            var isExp = false;

            //   number = [ minus ] int [ frac ] [ exp ]
            // => [sign] int [. int] [(e|E) int]

            // read sign
            if (*ptr == '-' || *ptr == '+')
            {
                // RFC7159 says sign is only for '-', but twitter sometime returns stupid JSON. 
                // So we also check '+' sign.
                ptr++;
            }

            // read main int 
            if (!SkipDigits(ref ptr, end))
            {
                throw factory.CreateException(ptr, "number is required after the sign.");
            }

            // read frac
            if (ptr <= end && *ptr == '.')
            {
                isFrac = true;
                if (!SkipDigits(ref ptr, end))
                {
                    throw factory.CreateException(ptr, "number is required after the decimal point.");
                }
            }

            // read exp
            if (ptr <= end && *ptr == 'e' || *ptr == 'E')
            {
                isExp = true;
                if (!SkipDigits(ref ptr, end))
                {
                    throw factory.CreateException(ptr, "number is required after the exponent sign.");
                }
            }

            var numstr = new String(begin, 0, (int)(ptr - begin));
            if (isFrac || isExp)
            {
                // this is floating point values
                return new JsonNumber(Double.Parse(numstr));
            }
            // this is integer
            return new JsonNumber(Int64.Parse(numstr));
        }

        // read values

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe JsonBoolean ReadTrue(ref char* ptr, char* end, JsonExceptionFactory factory)
        {
            Debug.Assert(*ptr == 't' || *ptr == 'T');
            ptr++;
            factory.Assert(ptr, end, 'r', 'R');
            ptr++;
            factory.Assert(ptr, end, 'u', 'U');
            ptr++;
            factory.Assert(ptr, end, 'e', 'E');
            ptr++;
            return JsonBoolean.True;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe JsonBoolean ReadFalse(ref char* ptr, char* end, JsonExceptionFactory factory)
        {
            Debug.Assert(*ptr == 'f' || *ptr == 'F');
            ptr++;
            factory.Assert(ptr, end, 'a', 'A');
            ptr++;
            factory.Assert(ptr, end, 'l', 'L');
            ptr++;
            factory.Assert(ptr, end, 's', 'S');
            ptr++;
            factory.Assert(ptr, end, 'e', 'E');
            ptr++;
            return JsonBoolean.True;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe JsonNull ReadNull(ref char* ptr, char* end, JsonExceptionFactory factory)
        {
            Debug.Assert(*ptr == 'n' || *ptr == 'N');
            ptr++;
            factory.Assert(ptr, end, 'u', 'U');
            ptr++;
            factory.Assert(ptr, end, 'l', 'L');
            ptr++;
            factory.Assert(ptr, end, 'l', 'L');
            ptr++;
            return JsonNull.Null;
        }

        // helper functions ------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void SkipWhitespaces(ref char* ptr, char* end)
        {
            while (ptr <= end && IsWhitespace(ptr))
            {
                ptr++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool IsWhitespace(char* c)
        {
            return *c == ' ' || *c == '\t' || *c == '\r' || *c == '\n';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool SkipDigits(ref char* ptr, char* end)
        {
            var begin = ptr;
            while (ptr <= end && IsDigit(ptr))
            {
                ptr++;
            }
            return begin != ptr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool IsDigit(char* c)
        {
            return *c >= '0' && *c <= '9';
        }

        private unsafe class JsonExceptionFactory
        {
            private readonly char* _begin;
            private readonly string _json;

            public JsonExceptionFactory(char* begin, string json)
            {
                _begin = begin;
                _json = json;
            }

            public JsonParseException CreateException(char* ptr, string message)
            {
                var index = ptr - _begin;
                if (index > _json.Length)
                {
                    index = _json.Length;
                }
                throw new JsonParseException(message, _json, index);
            }

            public void Assert(char* ptr, char c)
            {
                if (*ptr != c)
                {
                    CreateException(ptr, $"{c} is expected in this place, but placed char is {*ptr}.");
                }
            }
            public void Assert(char* ptr, char* end, char c)
            {
                if (ptr > end || *ptr != c)
                {
                    CreateException(ptr, $"{c} is expected in this place, but placed char is {*ptr}.");
                }
            }

            public void Assert(char* ptr, char c1, char c2)
            {
                if (*ptr != c1 && *ptr != c2)
                {
                    CreateException(ptr, $"{c1} or {c2} is expected in this place, but placed char is {*ptr}.");
                }
            }
            public void Assert(char* ptr, char* end, char c1, char c2)
            {
                if (ptr > end || *ptr != c1 && *ptr != c2)
                {
                    CreateException(ptr, $"{c1} or {c2} is expected in this place, but placed char is {*ptr}.");
                }
            }
        }
    }
}
