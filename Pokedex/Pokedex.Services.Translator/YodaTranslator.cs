using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Pokedex.Services.Translator
{
    public class YodaTranslator : BaseTranslator
    {
        public override string Name => TranslatorNames.YodaTranslatorName;

        public YodaTranslator(IHttpClientFactory clientFactory, ILoggerFactory loggerFactory, string url) : base(clientFactory, loggerFactory, url)
        {
        }
    }
}