using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pokedex.Common;
using Pokedex.Services.Translator;

namespace Pokedex.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly IPokemonProvider _pokemonProvider;
        private readonly ILogger<PokemonService> _logger;
        private readonly ITranslatorFactory _translatorFactory;

        public PokemonService(ITranslatorFactory translatorFactory, ILogger<PokemonService> logger, IPokemonProvider pokemonProvider)
        {
            _pokemonProvider = pokemonProvider;
            _translatorFactory = translatorFactory;
            _logger = logger;
        }

        public async Task<Pokemon> GetPokemonAsync(string name, bool withStandardDescription)
        {
            var pokemon = await _pokemonProvider.GetPokemonAsync(name);

            if (pokemon != null && !withStandardDescription)
            {
                pokemon.Description = await GetDescriptionAsync(pokemon);
            }

            return pokemon;
        }

        private async Task<string> GetDescriptionAsync(Pokemon pokemon)
        {
            if (string.IsNullOrWhiteSpace(pokemon.Description))
            {
                _logger.LogWarning($"Pokemon with name = {pokemon.Name} doesn't contain description.");
                return pokemon.Description;
            }

            var translator = GetSpecificTranslator(pokemon);

            return await translator.Translate(pokemon.Description);
        }

        private ITranslator GetSpecificTranslator(Pokemon pokemon)
        {
            if (pokemon.IsLegendary || pokemon.Habitat?.ToLower() == "cave")
            {
                return _translatorFactory.Create(TranslatorNames.YodaTranslatorName);
            }

            return _translatorFactory.Create(TranslatorNames.ShakespeareTranslatorName);
        }


    }
}
