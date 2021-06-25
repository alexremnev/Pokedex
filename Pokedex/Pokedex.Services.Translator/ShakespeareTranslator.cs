using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Pokedex.Services.Translator
{
    public class ShakespeareTranslator : BaseTranslator
    {
        public override string Name => TranslatorNames.ShakespeareTranslatorName;
        public ShakespeareTranslator(IHttpClientFactory clientFactory, ILoggerFactory loggerFactory, string url) : base(clientFactory, loggerFactory, url)
        {
        }
    }
}