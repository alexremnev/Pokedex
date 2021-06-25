using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Pokedex.Common;
using Pokedex.Services.Translator;
using Xunit;

namespace Pokedex.Services.Tests
{
    public class PokemonServiceTests
    {
        private readonly Mock<IPokemonProvider> _pokemonProvider;
        private readonly Mock<ITranslatorFactory> _translatorFactory;
        private readonly Mock<ILogger<PokemonService>> _logger;
        private readonly Mock<ITranslator> _translator;

        private readonly string _shakespeareTranslation = "shakespeare";
        private readonly string _yodaTranslation = "yoda";

        private readonly IDictionary<string, Pokemon> _pokemonStore = new Dictionary<string, Pokemon>
        {
            { "pokemon", new Pokemon
                {
                    Name = "pokemon",
                    Description = "text",
                    Habitat = "urban",
                    IsLegendary = false
                }
            },
            { "legendaryPokemon", new Pokemon
                {
                    Name = "legendaryPokemon",
                    Description = "text",
                    Habitat = "urban",
                    IsLegendary = true
                }
            },
            { "cavePokemon", new Pokemon
                {
                    Name = "cavePokemon",
                    Description = "text",
                    Habitat = "cave",
                    IsLegendary = false
                }
            },
    };

        public PokemonServiceTests()
        {
            _logger = new Mock<ILogger<PokemonService>>();
            _pokemonProvider = new Mock<IPokemonProvider>();
            _translatorFactory = new Mock<ITranslatorFactory>();
            _translator = new Mock<ITranslator>();

            _translatorFactory.Setup(p => p.Create(It.IsAny<string>())).Returns(_translator.Object);
        }

        [Fact]
        public async Task GivenPokemonName_WhenGetPokemonAsync_ThenReturnsPokemonResult()
        {
            // Arrange
            var pokemon = _pokemonStore["pokemon"];
            _pokemonProvider.Setup(p => p.GetPokemonAsync(It.IsAny<string>())).Returns(Task.FromResult(pokemon));
            var pokemonService = new PokemonService(_translatorFactory.Object, _logger.Object, _pokemonProvider.Object);

            // Act
            var pokemonResult = await pokemonService.GetPokemonAsync(pokemon.Name, true);

            // Assert
            Assert.Equal(pokemon.Name, pokemonResult.Name);
            Assert.Equal(pokemon.Description, pokemonResult.Description);
            Assert.Equal(pokemon.Habitat, pokemonResult.Habitat);
            Assert.Equal(pokemon.IsLegendary, pokemonResult.IsLegendary);
        }

        [Fact]
        public async Task GivenPokemonName_WhenGetPokemonAsync_ThenReturnsPokemonResultWithShakespeareTranslation()
        {
            // Arrange
            var pokemon = _pokemonStore["pokemon"];
            _translator.Setup(p => p.Translate(pokemon.Description)).Returns(Task.FromResult(_shakespeareTranslation));
            _pokemonProvider.Setup(p => p.GetPokemonAsync(It.IsAny<string>())).Returns(Task.FromResult(pokemon));
            var pokemonService = new PokemonService(_translatorFactory.Object, _logger.Object, _pokemonProvider.Object);

            // Act
            var pokemonResult = await pokemonService.GetPokemonAsync(pokemon.Name, false);

            // Assert
            Assert.Equal(pokemon.Name, pokemonResult.Name);
            Assert.Equal(_shakespeareTranslation, pokemonResult.Description);
            Assert.Equal(pokemon.Habitat, pokemonResult.Habitat);
            Assert.Equal(pokemon.IsLegendary, pokemonResult.IsLegendary);
        }

        [Theory]
        [InlineData("cavePokemon")]
        [InlineData("legendaryPokemon")]
        public async Task GivenPokemonName_WhenGetPokemonAsync_ThenReturnsPokemonResultWithYodaTranslation(string pokemonName)
        {
            // Arrange
            var pokemon = _pokemonStore[pokemonName];
            _translator.Setup(p => p.Translate(pokemon.Description)).Returns(Task.FromResult(_yodaTranslation));
            _pokemonProvider.Setup(p => p.GetPokemonAsync(It.IsAny<string>())).Returns(Task.FromResult(pokemon));
            var pokemonService = new PokemonService(_translatorFactory.Object, _logger.Object, _pokemonProvider.Object);

            // Act
            var pokemonResult = await pokemonService.GetPokemonAsync(pokemon.Name, false);

            // Assert

            Assert.Equal(pokemon.Name, pokemonResult.Name);
            Assert.Equal(_yodaTranslation, pokemonResult.Description);
            Assert.Equal(pokemon.Habitat, pokemonResult.Habitat);
            Assert.Equal(pokemon.IsLegendary, pokemonResult.IsLegendary);
        }

        [Fact]
        public async Task GivenPokemonName_WhenGetPokemonAsync_AndDescriptionIsNull_ThenReturnsPokemonResultWithDefaultTranslation()
        {
            // Arrange
            var pokemon = _pokemonStore["pokemon"];
            pokemon.Description = null;
            _pokemonProvider.Setup(p => p.GetPokemonAsync(It.IsAny<string>())).Returns(Task.FromResult(pokemon));
            var pokemonService = new PokemonService(_translatorFactory.Object, _logger.Object, _pokemonProvider.Object);

            // Act
            var pokemonResult = await pokemonService.GetPokemonAsync(pokemon.Name, false);

            // Assert
            _translatorFactory.Verify(mock => mock.Create(It.IsAny<string>()), Times.Never);
            _translator.Verify(mock => mock.Translate(It.IsAny<string>()), Times.Never);
            Assert.Equal(pokemon.Name, pokemonResult.Name);
            Assert.Null(pokemonResult.Description);
            Assert.Equal(pokemon.Habitat, pokemonResult.Habitat);
            Assert.Equal(pokemon.IsLegendary, pokemonResult.IsLegendary);
        }
    }
}
