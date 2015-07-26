
namespace Cadena
{
    public interface IApiAccess
    {
        IOAuthCredential Credential { get; }

        IApiAccessConfiguration AccessConfiguration { get; }

        IProxyConfiguration ProxyConfiguration { get; }
    }
}
