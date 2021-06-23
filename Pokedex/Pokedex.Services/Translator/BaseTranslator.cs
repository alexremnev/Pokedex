using System.Net.Http;
using System.Threading.Tasks;
using Pokedex.Services.Exceptions;
using Pokedex.Services.Models;
using static Pokedex.Services.Utils;

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

        public async Task<string> Translate(string description)
        {
            var result = await _httpClient.PostAsync($"{_url}", ConvertToStringContent(new { text = description }));

            if (!result.IsSuccessStatusCode)
            {
                throw new ServiceUnavailableException($"Can not translate using {Name} translator, {result.ReasonPhrase}");
            }

            var translationResult = await ReadResultAsync<TranslationResult>(result);
            return translationResult?.Contents?.Translated;
        }
    }
}