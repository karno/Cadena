using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Streams;
using Cadena.Engine._Internals;

namespace Cadena.Engine.StreamReceivers
{
    public sealed class UserStreamReceiver : StreamReceiverBase
    {
        private readonly IApiAccess _access;
        private readonly IStreamHandler _handler;

        private readonly TimeSpan _userStreamTimeout = TimeSpan.FromSeconds(70);

        private bool _disposed;

        public UserStreamReceiver(ReceiveManager manager, IApiAccess access, IStreamHandler handler) : base(manager)
        {
            _access = access;
            _handler = handler;
        }

        private async Task Connect(CancellationToken cancellationToken)
        {
            try
            {
                await UserStreams.Connect(_access, ParseLine, _userStreamTimeout, cancellationToken);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        private void ParseLine(string json)
        {
            UserStreamParser.ParseStreamLine(json, _handler);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}
