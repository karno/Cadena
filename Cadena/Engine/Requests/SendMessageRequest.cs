using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Data;
using Cadena.Twitter.Parameters;
using Cadena.Twitter.Rest;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public class SendMessageRequest : RequestBase<IApiResult<TwitterStatus>>
    {
        [NotNull]
        public ApiAccessor Accessor { get; }

        [NotNull]
        public UserParameter Recipient { get; }

        [NotNull]
        public string Text { get; }

        public SendMessageRequest([NotNull] ApiAccessor accessor, [NotNull] UserParameter recipient, [NotNull] string text)
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
