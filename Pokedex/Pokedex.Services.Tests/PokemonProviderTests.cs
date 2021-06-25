using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Pokedex.Common.Exceptions;
using Xunit;

namespace Pokedex.Services.Tests
{
    public class PokemonProviderTests
    {
        private const string ExpectedUrl = "http://pokemon.test.com";
        private const string ExpectedName = "ditto";

        private readonly Mock<ILogger<PokemonProvider>> _logger;
        private readonly Mock<IHttpClientFactory> _httpClientFactory;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

        public PokemonProviderTests()
        {
            _logger = new Mock<ILogger<PokemonProvider>>();

            _httpClientFactory = new Mock<IHttpClientFactory>();

            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.ToString().Equals($"{ExpectedUrl}/{ExpectedName}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(

                        "{'name':'ditto', 'habitat': { 'name' : 'urban' }, 'flavor_text_entries': [{'flavor_text': 'Description\n.', 'language': {'name': 'en','url': 'https://url'},}], 'is_legendary': true}")
                });
            var client = new HttpClient(_mockHttpMessageHandler.Object);
            _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        }

        [Fact]
        public async Task GivenPokemonName_WhenGetPokemonAsync_ThenReturnsPokemonResult()
        {
            // Arrange
            var pokemonProvider = new PokemonProvider(_httpClientFactory.Object, _logger.Object, ExpectedUrl);

            // Act
            var pokemon = await pokemonProvider.GetPokemonAsync(ExpectedName);

            // Assert
            Assert.Equal(pokemon.Name, ExpectedName.ToLower());
            Assert.Equal("Description .", pokemon.Description);
            Assert.Equal("urban", pokemon.Habitat);
            Assert.True(pokemon.IsLegendary);
        }

        [Fact]
        public async Task GivenInvalidPokemonName_WhenGetPokemonAsync_ThenReturnsNull()
        {
            // Arrange
            var expectedName = "";
            var pokemonProvider = new PokemonProvider(_httpClientFactory.Object, _logger.Object, ExpectedUrl);

            // Act
            var pokemon = await pokemonProvider.GetPokemonAsync(expectedName);

            // Assert
            Assert.Null(pokemon);
        }

        [Fact]
        public async Task GivenPokemonName_WhenGetPokemonAsyncAndPokemonNotFound_ThenReturnsNull()
        {

            // Arrange
            var notExistedPokemonName = "pokemon";
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.ToString().Equals($"{ExpectedUrl}/{notExistedPokemonName}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = null
                });

            var pokemonProvider = new PokemonProvider(_httpClientFactory.Object, _logger.Object, ExpectedUrl);

            // Act
            var pokemon = await pokemonProvider.GetPokemonAsync(notExistedPokemonName);

            // Assert
            Assert.Null(pokemon);
        }

        [Fact]
        public async Task GivenPokemonName_WhenGetPokemonAsyncAndInternalErrorOccured_ThenReturnsException()
        {
            // Arrange
            var pokemonName = "pokemon";
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.ToString().Equals($"{ExpectedUrl}/{pokemonName}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = null
                });

            var pokemonProvider = new PokemonProvider(_httpClientFactory.Object, _logger.Object, ExpectedUrl);

            // Act
            var exception = await Assert.ThrowsAsync<ServiceUnavailableException>(() => pokemonProvider.GetPokemonAsync(pokemonName));

            // Assert
            Assert.Equal($"Can not get a Pokemon with the name = {pokemonName}.", exception.Message);
        }

        [Fact]
        public async Task GivenPokemonName_WhenGetPokemonAsync_ThenReturnsPokemonResultWithFirstENLang()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.ToString().Equals($"{ExpectedUrl}/{ExpectedName}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(

                        "{'name':'ditto', 'habitat': { 'name' : 'urban' }, 'flavor_text_entries': [{'flavor_text': 'Описание.', 'language': {'name': 'ru','url': 'https://url/1'}},{'flavor_text': 'Description.', 'language': {'name': 'en','url': 'https://url'}}], 'is_legendary': true}")
                });
            var pokemonProvider = new PokemonProvider(_httpClientFactory.Object, _logger.Object, ExpectedUrl);

            // Act
            var pokemon = await pokemonProvider.GetPokemonAsync(ExpectedName);

            // Assert
            Assert.Equal(pokemon.Name, ExpectedName.ToLower());
            Assert.Equal("Description.", pokemon.Description);
            Assert.Equal("urban", pokemon.Habitat);
            Assert.True(pokemon.IsLegendary);
        }

        [Fact]
        public async Task GivenPokemonName_WhenGetPokemonAsync_AndDescriptionInEngIsNotFound_ThenReturnsPokemonResultWithEmptyDescription()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.ToString().Equals($"{ExpectedUrl}/{ExpectedName}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(

                        "{'name':'ditto', 'habitat': { 'name' : 'urban' }, 'flavor_text_entries': [{'flavor_text': 'Описание.', 'language': {'name': 'ru','url': 'https://url/1'}},{'flavor_text': 'Beschreibung.', 'language': {'name': 'de','url': 'https://url'}}], 'is_legendary': true}")
                });
            var pokemonProvider = new PokemonProvider(_httpClientFactory.Object, _logger.Object, ExpectedUrl);

            // Act
            var pokemon = await pokemonProvider.GetPokemonAsync(ExpectedName);

            // Assert
            Assert.Equal(pokemon.Name, ExpectedName.ToLower());
            Assert.Null(pokemon.Description);
            Assert.Equal("urban", pokemon.Habitat);
            Assert.True(pokemon.IsLegendary);
        }
    }
}
