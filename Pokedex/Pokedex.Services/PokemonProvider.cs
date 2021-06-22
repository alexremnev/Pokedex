using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pokedex.Services.Models;

namespace Pokedex.Services
{
    public class PokemonProvider : IPokemonProvider
    {
        private readonly IAppConfiguration _appConfiguration;
        private readonly HttpClient _httpClient;
        public PokemonProvider(IAppConfiguration appConfiguration, HttpClient httpClient)
        {
            _appConfiguration = appConfiguration;
            _httpClient = httpClient;
        }

        public async Task<PokemonDetails> GetPokemonAsync(string name)
        {
            var response = await _httpClient.GetAsync($"{_appConfiguration.ApiEndpoint}/{name}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Can not get a Pokemon with the name = {name}. {response.ReasonPhrase}");
            }

            return await ReadResultAsync<PokemonDetails>(response);

        }

        private static async Task<T> ReadResultAsync<T>(HttpResponseMessage responseMessage)
        {
            var stringResult = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringResult);
        }
    }
}
