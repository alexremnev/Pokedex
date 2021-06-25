using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pokedex.Common;
using Pokedex.Services.Translator.Models;
using static Pokedex.Common.Utils;

namespace Pokedex.Services.Translator
{
    public abstract class BaseTranslator : ITranslator
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _url;

        public abstract string Name { get; }

        protected BaseTranslator(IHttpClientFactory clientFactory, ILoggerFactory loggerFactory, string url)
        {
            _httpClient = clientFactory.CreateClient();
            _logger = loggerFactory.CreateLogger(GetType());
            _url = url;
        }

        public async Task<string> Translate(string value)
        {
            var result = await _httpClient.PostAsync(_url, content: ConvertToStringContent(new { text = value }));

            if (!result.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Can not translate using {Name} translator, {result.ReasonPhrase}");
                return value;
            }

            var translationResult = await result.ReadResultAsync<TranslationResult>();
            return translationResult?.Contents?.Translated;
        }
    }
}