using System;

namespace Cadena.Meteor
{
    internal static class MeteorJsonHelper
    {
        public static unsafe string UnsafeUnescape(string jsonString, int decodedLength)
        {
            var len = jsonString.Length;
            var buf = new char[decodedLength];
            fixed (char* bufptr = buf)
            fixed (char* srcptr = jsonString)
            {
                var bp = bufptr;
                for (var sp = srcptr; sp < srcptr + len; sp++)
                {
                    if (*sp == '\\')
                    {
                        sp++;
                        switch (*sp)
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
                                    sp++;
                                    code <<= 4;
                                    if (*sp <= '9' && *sp >= '0')
                                    {
                                        code += *sp - '0';
                                    }
                                    else if (*sp <= 'F' && *sp >= 'A')
                                    {
                                        // code += *sp - 'A' + 10
                                        code += *sp - '7';
                                    }
                                    else if (*sp <= 'f' && *sp >= 'a')
                                    {
                                        // code += *sp - 'a' + 10
                                        code += *sp - 'W';
                                    }
                                    else
                                    {
                                        // invalid code, abort processing
                                        sp -= i;
                                        goto default;
                                    }
                                }
                                if (code > Char.MaxValue)
                                {
                                    // out of range
                                    sp -= 4;
                                    goto default;
                                }
                                *bp = (char)code;
                                break;
                            default:
                                // this is not registered escape code.
                                sp--;
                                *bp = *sp;
                                break;
                        }
                    }
                    else
                    {
                        *bp = *sp;
                    }
                    bp++;
                }
            }
            return new string(buf);
        }
    }
}
