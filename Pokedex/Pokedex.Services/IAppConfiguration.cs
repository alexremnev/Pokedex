namespace Pokedex.Services
{
    public class AppConfiguration : IAppConfiguration
    {
        public string ApiEndpoint { get; set; }

        public string ShakespeareTranslatorUrl { get; set; }
        public string YodaTranslatorUrl { get; set; }
    }

    public interface IAppConfiguration
    {
        string ApiEndpoint { get; set; }
        string ShakespeareTranslatorUrl { get; set; }
        string YodaTranslatorUrl { get; set; }
    }
}
