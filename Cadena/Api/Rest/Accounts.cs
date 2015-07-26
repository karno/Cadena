using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internal;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Api.Rest
{
    public static class Accounts
    {
        #region account/verify_accesss

        public static async Task<IApiResult<TwitterUser>> VerifyCredentialAsync(
            [NotNull] this IApiAccess access,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object>
            {
                {"skip_status", true}
            };

            return await access.GetAsync("account/verify_accesss.json",
                param, ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region account/update_profile

        public static async Task<IApiResult<TwitterUser>> UpdateProfileAsync(
            [NotNull] this IApiAccess access, [CanBeNull] string name, [CanBeNull] string url,
            [CanBeNull] string location, [CanBeNull] string description,
            CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            var param = new Dictionary<string, object>
            {
                {"name", name},
                {"url", url},
                {"location", location},
                {"description", description},
                {"skip_status", true},
            };
            return await access.PostAsync("account/update_profile.json",
                param, ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region account/update_profile_image

        public static async Task<IApiResult<TwitterUser>> UpdateProfileImageAsync(
            [NotNull] this IApiAccess access,
            [NotNull] byte[] image, CancellationToken cancellationToken)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (image == null) throw new ArgumentNullException(nameof(image));
            var content = new MultipartFormDataContent
            {
                {new StringContent("true"), "skip_status"},
                {new ByteArrayContent(image), "image", "image.png"}
            };
            return await access.PostAsync("account/update_profile_image.json",
                content, ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}