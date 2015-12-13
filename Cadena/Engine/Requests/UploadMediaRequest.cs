using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class UploadMediaRequest : RequestBase<IApiResult<TwitterUploadedMedia>>
    {
        [NotNull]
        public IApiAccess Access { get; }

        [NotNull]
        public byte[] Media { get; }

        [CanBeNull]
        public long[] AdditionalOwnerIds { get; }

        [CanBeNull]
        public IProgress<double> SentPercentageCallback { get; }

        public UploadMediaRequest([NotNull] IApiAccess access, [NotNull] IEnumerable<byte> media,
            [CanBeNull] IEnumerable<IApiAccess> additionalOwners = null,
            [CanBeNull] IProgress<double> sentPercentageCallback = null)
            : this(access, media, additionalOwners?.Select(a => a.Credential.Id), sentPercentageCallback)
        {
        }


        public UploadMediaRequest([NotNull] IApiAccess access, [NotNull] IEnumerable<byte> media,
            [CanBeNull] IEnumerable<long> additionalOwnerIds = null,
            [CanBeNull] IProgress<double> sentPercentageCallback = null)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (media == null) throw new ArgumentNullException(nameof(media));
            AdditionalOwnerIds = additionalOwnerIds?.ToArray();
            Access = access;
            SentPercentageCallback = sentPercentageCallback;
            Media = media.ToArray();
        }

        public override Task<IApiResult<TwitterUploadedMedia>> Send(CancellationToken token)
        {
            // upload chunked per 256KB.
            var chunkSize = 256 * 1024;
            var filetype = MediaFileUtility.GetMediaType(Media);
            if (filetype == SupportedMediaTypes.Unknown)
            {
                throw new ArgumentException("This media is not supported.");
            }
            var totalBytes = Media.Length;
            var callback = new Progress<int>(i => SentPercentageCallback?.Report((double)i / totalBytes));
            var mime = MediaFileUtility.GetMime(filetype);
            return Access.UploadLargeMediaAsync(Media, mime, AdditionalOwnerIds, chunkSize, callback, token);
        }
    }
}
