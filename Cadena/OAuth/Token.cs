/************************************************************
The MIT License (MIT)
Copyright 2013-2014 neuecc, AsyncOAuth project
Copyright 2016 Karno

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
************************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Cadena.OAuth
{
    /// <summary>represents OAuth Token</summary>
    [DebuggerDisplay("Key = {Key}, Secret = {Secret}")]
    [DataContract]
    public abstract class Token
    {
        [DataMember(Order = 1)]
        public string Key { get; }
        [DataMember(Order = 2)]
        public string Secret { get; }

        /// <summary>for serialize.</summary>
        [Obsolete("this is used for serialize")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Token()
        {

        }

        public Token([NotNull] string key, [NotNull] string secret)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (secret == null) throw new ArgumentNullException(nameof(secret));

            Key = key;
            Secret = secret;
        }
    }

    /// <summary>represents OAuth AccessToken</summary>
    [DataContract]
    public class AccessToken : Token
    {
        /// <summary>for serialize.</summary>
        [Obsolete("this is used for serialize")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AccessToken()
        {

        }

        public AccessToken(string key, string secret)
            : base(key, secret)
        { }
    }

    /// <summary>represents OAuth RequestToken</summary>
    [DataContract]
    public class RequestToken : Token
    {
        /// <summary>
        /// for serialize.
        /// </summary>
        [Obsolete("this is used for serialize")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public RequestToken()
        {

        }

        public RequestToken(string key, string secret)
            : base(key, secret)
        { }
    }
}