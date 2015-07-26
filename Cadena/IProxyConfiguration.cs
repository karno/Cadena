using System;
using System.Net;

namespace Cadena
{
    public interface IProxyConfiguration
    {
        bool UseSystemproxy { get; }

        Func<IWebProxy> ProxyProvider { get; }
    }
}
