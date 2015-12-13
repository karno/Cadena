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
        public IApiAccess Access { get; }

        [NotNull]
        public UserParameter Target { get; }

        public Relations Relation { get; }

        public UpdateRelationRequest([NotNull] IApiAccess access,
            [NotNull] UserParameter target, Relations relation)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (target == null) throw new ArgumentNullException(nameof(target));
            Access = access;
            Target = target;
            Relation = relation;
        }

        public override Task<IApiResult<TwitterUser>> Send(CancellationToken token)
        {
            switch (Relation)
            {
                case Relations.Follow:
                    return Access.CreateFriendshipAsync(Target, token);
                case Relations.Unfollow:
                    return Access.DestroyFriendshipAsync(Target, token);
                case Relations.Block:
                    return Access.CreateBlockAsync(Target, token);
                case Relations.ReportAsSpam:
                    return Access.ReportSpamAsync(Target, token);
                case Relations.Unblock:
                    return Access.DestroyBlockAsync(Target, token);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


    public enum Relations
    {
        Follow,
        Unfollow,
        Block,
        ReportAsSpam,
        Unblock
    }
}
