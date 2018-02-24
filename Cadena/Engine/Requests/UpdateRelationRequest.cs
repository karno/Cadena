using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class UpdateRelationRequest : RequestBase<IApiResult<TwitterUser>>
    {
        [NotNull]
        public IApiAccessor Accessor { get; }

        [NotNull]
        public UserParameter Target { get; }

        public Relationships Relation { get; }

        public UpdateRelationRequest([NotNull] IApiAccessor accessor,
            [NotNull] UserParameter target, Relationships relation)
        {
            Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Relation = relation;
        }

        public override Task<IApiResult<TwitterUser>> Send(CancellationToken token)
        {
            switch (Relation)
            {
                case Relationships.Follow:
                    return Accessor.CreateFriendshipAsync(Target, token);

                case Relationships.Unfollow:
                    return Accessor.DestroyFriendshipAsync(Target, token);

                case Relationships.Block:
                    return Accessor.CreateBlockAsync(Target, token);

                case Relationships.ReportAsSpam:
                    return Accessor.ReportSpamAsync(Target, token);

                case Relationships.Unblock:
                    return Accessor.DestroyBlockAsync(Target, token);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum Relationships
    {
        Follow,
        Unfollow,
        Block,
        ReportAsSpam,
        Unblock
    }
}