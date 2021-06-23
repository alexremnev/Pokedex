using System.Net.Http;
using System.Threading.Tasks;
using Pokedex.Common;
using Pokedex.Common.Exceptions;
using Pokedex.Services.Translator.Models;
using static Pokedex.Common.Utils;

namespace Pokedex.Services.Translator
{
    public abstract class BaseTranslator : ITranslator
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public abstract string Name { get; }

        protected BaseTranslator(HttpClient httpClient, string url)
        {
            _httpClient = httpClient;
            _url = url;
        }

        public async Task<string> Translate(string value)
        {
            var result = await _httpClient.PostAsync($"{_url}", content: ConvertToStringContent(new { text = value }));

            if (!result.IsSuccessStatusCode)
            {
                throw new ServiceUnavailableException($"Can not translate using {Name} translator, {result.ReasonPhrase}");
            }

            var translationResult = await result.ReadResultAsync<TranslationResult>();
            return translationResult?.Contents?.Translated;
        }
    }
}