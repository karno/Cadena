﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class DeleteStatusRequest : RequestBase<IApiResult<TwitterStatus>>
    {
        [NotNull]
        public IApiAccessor Accessor { get; }

        public long Id { get; }

        public StatusType Type { get; }

        public DeleteStatusRequest([NotNull] IApiAccessor accessor, long id, StatusType type)
        {
            Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            Id = id;
            Type = type;
        }

        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            return Type == StatusType.Tweet
                ? Accessor.DestroyAsync(Id, token)
                : Accessor.DestroyDirectMessageAsync(Id, token);
        }
    }
}