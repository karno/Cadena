// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//
// ! This file is a part of assets of AsyncOAuth project.
//
// AsyncOAuth - https://github.com/neuecc/AsyncOAuth from @neuecc
// This file is licensed under the MIT License. Check above link for more detail.
//
// Note: Several modifications may have been applied from original source by @ karno.
//
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System.Linq;

namespace AsyncOAuth
{
    /// <summary>OAuth Response</summary>
    public class TokenResponse<T> where T : Token
    {
        public T Token { get; private set; }
        public ILookup<string, string> ExtraData { get; private set; }

        public TokenResponse(T token, ILookup<string, string> extraData)
        {
            Precondition.NotNull(token, "token");
            Precondition.NotNull(extraData, "extraData");

            this.Token = token;
            this.ExtraData = extraData;
        }
    }
}