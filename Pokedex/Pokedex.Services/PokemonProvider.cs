using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
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
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            _logger.LogInformation($"Getting a Pokemon with the name = {name}.");
            var pokemonResponse = await _httpClient.GetAsync($"{_apiEndpoint}/{name.ToLower()}");

            if (!pokemonResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"Can not get a Pokemon with the name = {name}. {pokemonResponse.ReasonPhrase}");

                if (pokemonResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw new ServiceUnavailableException($"Can not get a Pokemon with the name = {name}.");
            }

            var pokemonDetails = await pokemonResponse.ReadResultAsync<PokemonDetails>();
            var pokemon = ConvertToPokemon(pokemonDetails);

            CleanDescription(pokemon);
            return pokemon;
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

        private static void CleanDescription(Pokemon pokemon)
        {
            if (pokemon.Description != null)
            {
                pokemon.Description = Regex.Replace(pokemon.Description, @"(\n|\t|\f)", " ");
            }
        }
    }
}