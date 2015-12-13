using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class UpdateFriendshipRequest : RequestBase<IApiResult<TwitterFriendship>>
    {
        [NotNull]
        public IApiAccess Access { get; }

        [NotNull]
        public UserParameter TargetUser { get; }

        public bool? DeviceNotifications { get; }

        public bool? ShowRetweets { get; }

        public UpdateFriendshipRequest([NotNull] IApiAccess access, [NotNull] UserParameter param,
            bool? deviceNotifications, bool? showRetweets)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (deviceNotifications == null && showRetweets == null)
            {
                throw new ArgumentException("deviceNotifications or showRetweets must be specified.");
            }
            Access = access;
            TargetUser = param;
            DeviceNotifications = deviceNotifications;
            ShowRetweets = showRetweets;
        }

        public override Task<IApiResult<TwitterFriendship>> Send(CancellationToken token)
        {
            return Access.UpdateFriendshipAsync(TargetUser, DeviceNotifications, ShowRetweets, token);
        }
    }
}
