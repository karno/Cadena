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
        public IApiAccessor Accessor { get; }

        [NotNull]
        public byte[] Media { get; }

        [CanBeNull]
        public long[] AdditionalOwnerIds { get; }

        [CanBeNull]
        public IProgress<double> SentPercentageCallback { get; }

        public UploadMediaRequest([NotNull] IApiAccessor accessor, [NotNull] IEnumerable<byte> media,
            [CanBeNull] IEnumerable<IApiAccessor> additionalOwners = null,
            [CanBeNull] IProgress<double> sentPercentageCallback = null)
            : this(accessor, media, additionalOwners?.Select(a => a.Id), sentPercentageCallback)
        {
        }


        public UploadMediaRequest([NotNull] IApiAccessor accessor, [NotNull] IEnumerable<byte> media,
            [CanBeNull] IEnumerable<long> additionalOwnerIds = null,
            [CanBeNull] IProgress<double> sentPercentageCallback = null)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (media == null) throw new ArgumentNullException(nameof(media));
            AdditionalOwnerIds = additionalOwnerIds?.ToArray();
            Accessor = accessor;
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
            return Accessor.UploadLargeMediaAsync(Media, mime, AdditionalOwnerIds, chunkSize, callback, token);
        }
    }
}
