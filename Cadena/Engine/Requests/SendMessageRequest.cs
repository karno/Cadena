using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class SendMessageRequest : RequestBase<IApiResult<TwitterStatus>>
    {
        [NotNull]
        public IApiAccess Access { get; }

        [NotNull]
        public UserParameter Recipient { get; }

        [NotNull]
        public string Text { get; }

        public SendMessageRequest([NotNull] IApiAccess access, [NotNull] UserParameter recipient, [NotNull] string text)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (recipient == null) throw new ArgumentNullException(nameof(recipient));
            if (text == null) throw new ArgumentNullException(nameof(text));
            Access = access;
            Recipient = recipient;
            Text = text;
        }

        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            return Access.SendDirectMessageAsync(Recipient, Text, token);
        }
    }
}
