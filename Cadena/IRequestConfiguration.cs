
namespace Cadena
{
    /// <summary>
    /// Describe basic Configuration for accessing API.
    /// </summary>
    public interface IRequestConfiguration
    {
        string Endpoint { get; }

        string UserAgent { get; }

        bool UseGZip { get; }
    }
}
