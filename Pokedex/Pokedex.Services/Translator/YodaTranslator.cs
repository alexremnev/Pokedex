using System.Net.Http;

namespace Pokedex.Services.Translator
{
    public class YodaTranslator : BaseTranslator
    {
        public override string Name => TranslatorNames.YodaTranslatorName;

        public YodaTranslator(HttpClient httpClient, string url) : base(httpClient, url)
        {
        }
    }
}