using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Data;
using Cadena._Internals;
using JetBrains.Annotations;

namespace Cadena.Api.Rest
{
    public static class Accounts
    {
        #region account/verify_accesss

        public static async Task<IApiResult<TwitterUser>> VerifyCredentialAsync(
            [NotNull] this IApiAccessor accessor,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"skip_status", true}
            };

            return await accessor.GetAsync("account/verify_accesss.json",
                param, ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion account/verify_accesss

        #region account/update_profile

        public static async Task<IApiResult<TwitterUser>> UpdateProfileAsync(
            [NotNull] this IApiAccessor accessor, [CanBeNull] string name, [CanBeNull] string url,
            [CanBeNull] string location, [CanBeNull] string description,
            CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var param = new Dictionary<string, object>
            {
                {"name", name},
                {"url", url},
                {"location", location},
                {"description", description},
                {"skip_status", true},
            };
            return await accessor.PostAsync("account/update_profile.json",
                param, ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion account/update_profile

        #region account/update_profile_image

        public static async Task<IApiResult<TwitterUser>> UpdateProfileImageAsync(
            [NotNull] this IApiAccessor accessor,
            [NotNull] byte[] image, CancellationToken cancellationToken)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (image == null) throw new ArgumentNullException(nameof(image));
            var content = new MultipartFormDataContent
            {
                {new StringContent("true"), "skip_status"},
                {new ByteArrayContent(image), "image", "image.png"}
            };
            return await accessor.PostAsync("account/update_profile_image.json",
                content, ResultHandlers.ReadAsUserAsync, cancellationToken).ConfigureAwait(false);
        }

        #endregion account/update_profile_image
    }
}