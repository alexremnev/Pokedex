using System.Net.Http;

namespace Pokedex.Services.Translator
{
    public class ShakespeareTranslator : BaseTranslator
    {
        public override string Name => TranslatorNames.ShakespeareTranslatorName;
        public ShakespeareTranslator(HttpClient httpClient, string url) : base(httpClient, url)
        {
        }
    }
}