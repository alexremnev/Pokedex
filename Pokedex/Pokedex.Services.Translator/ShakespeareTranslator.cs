using System.Net.Http;

namespace Pokedex.Services.Translator
{
    public class ShakespeareTranslator : BaseTranslator
    {
        public override string Name => TranslatorNames.ShakespeareTranslatorName;
        public ShakespeareTranslator(IHttpClientFactory clientFactory, string url) : base(clientFactory, url)
        {
        }
    }
}