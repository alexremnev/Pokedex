using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pokedex.Common;
using Pokedex.Common.Exceptions;
using Pokedex.Services.Models;

namespace Pokedex.Services
{
    public class PokemonProvider : IPokemonProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PokemonProvider> _logger;
        private readonly string _apiEndpoint;
        private const string SupportedLanguage = "en";

        public PokemonProvider(IHttpClientFactory clientFactory, ILogger<PokemonProvider> logger, string apiEndpoint)
        {
            _httpClient = clientFactory.CreateClient();
            _logger = logger;
            _apiEndpoint = apiEndpoint;
        }

        public async Task<Pokemon> GetPokemonAsync(string name)
        {
            _logger.LogInformation($"Getting a Pokemon with name = {name}.");
            var pokemonDetailsResponse = await _httpClient.GetAsync($"{_apiEndpoint}/{name.ToLower()}");

            if (!pokemonDetailsResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"Can not get a Pokemon with the name = {name}. {pokemonDetailsResponse.ReasonPhrase}");

                if (pokemonDetailsResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw new ServiceUnavailableException($"Can not get a Pokemon with the name = {name}.");
            }

            var pokemonDetails = await pokemonDetailsResponse.ReadResultAsync<PokemonDetails>();
            return ConvertToPokemon(pokemonDetails);
        }

        private static Pokemon ConvertToPokemon(PokemonDetails pokemonDetails)
        {
            return new Pokemon
            {
                Name = pokemonDetails.Name,
                Description = pokemonDetails.FlavorTextEntries?.FirstOrDefault(x => x.Language?.Name == SupportedLanguage)?.Description,
                Habitat = pokemonDetails.Habitat?.Name,
                IsLegendary = pokemonDetails.IsLegendary
            };
        }
    }
}