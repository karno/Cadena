﻿using System;
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
        public IApiAccessor Accessor { get; }

        [NotNull]
        public UserParameter TargetUser { get; }

        public bool? DeviceNotifications { get; }

        public bool? ShowRetweets { get; }

        public UpdateFriendshipRequest([NotNull] IApiAccessor accessor, [NotNull] UserParameter param,
            bool? deviceNotifications, bool? showRetweets)
        {
            if (deviceNotifications == null && showRetweets == null)
            {
                throw new ArgumentException("deviceNotifications or showRetweets must be specified.");
            }
            Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            TargetUser = param ?? throw new ArgumentNullException(nameof(param));
            DeviceNotifications = deviceNotifications;
            ShowRetweets = showRetweets;
        }

        public override Task<IApiResult<TwitterFriendship>> Send(CancellationToken token)
        {
            return Accessor.UpdateFriendshipAsync(TargetUser, DeviceNotifications, ShowRetweets, token);
        }
    }
}