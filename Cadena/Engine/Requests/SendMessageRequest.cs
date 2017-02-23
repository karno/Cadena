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
        public IApiAccessor Accessor { get; }

        [NotNull]
        public UserParameter Recipient { get; }

        [NotNull]
        public string Text { get; }

        public SendMessageRequest([NotNull] IApiAccessor accessor, [NotNull] UserParameter recipient, [NotNull] string text)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (recipient == null) throw new ArgumentNullException(nameof(recipient));
            if (text == null) throw new ArgumentNullException(nameof(text));
            Accessor = accessor;
            Recipient = recipient;
            Text = text;
        }

        public override Task<IApiResult<TwitterStatus>> Send(CancellationToken token)
        {
            return Accessor.SendDirectMessageAsync(Recipient, Text, token);
        }
    }
}
