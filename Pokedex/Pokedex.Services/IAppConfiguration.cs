using System.Runtime.CompilerServices;

namespace Pokedex.Services
{
    public class AppConfiguration : IAppConfiguration
    {
        public string ApiEndpoint { get; set; }
    }

    public interface IAppConfiguration
    {
        string ApiEndpoint { get; set; }
    }
}
