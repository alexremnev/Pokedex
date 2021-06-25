using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LightBDD.Framework;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Pokedex.Common;
using Xunit;

namespace Pokedex.Api.Tests
{
    public class Context
    {
        private readonly TestServer _testServer;
        private HttpResponseMessage _response;

        private string _pokemonName;

        private const string ApiUrl = "pokemon";
        public Context()
        {
            _testServer = new TestServer(new WebHostBuilder()
                .UseConfiguration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile("appsettings.Development.json", false, true)
                    .Build())
                .UseStartup<Startup>());
        }

        #region Given
        public void Given_a_valid_pokemon_name()
        {
            _pokemonName = "ditto";
        }

        public void Given_a_legendary_pokemon_name()
        {
            _pokemonName = "cresselia";
        }

        public void Given_a_invalid_pokemon_name()
        {
            _pokemonName = "invalid";
        }
        #endregion

        #region When

        public async Task When_translated_pokemon_is_requested()
        {
            var client = GetHttpClient();
            _response = await client.GetAsync($"{ApiUrl}/translated/{_pokemonName}");
        }

        public async Task When_pokemon_is_requested()
        {
            var client = GetHttpClient();
            _response = await client.GetAsync($"{ApiUrl}/{_pokemonName}");
        }

        #endregion

        #region Then

        [MultiAssert]
        public void Then_return_proper_response(HttpStatusCode httpStatusCode)
        {
            Assert.NotNull(_response);

            Assert.Equal(httpStatusCode, _response.StatusCode);
        }

        [MultiAssert]
        public async Task Then_return_proper_model(string habitat, bool isLegendary)
        {
            var model = await _response.Content.ReadAsJsonAsync<Pokemon>();
            Assert.Equal(_pokemonName, model.Name);
            Assert.NotNull(model.Description);
            Assert.Equal(habitat, model.Habitat);
            Assert.Equal(isLegendary, model.IsLegendary);
        }

        #endregion

        private HttpClient GetHttpClient()
        {
            return _testServer.CreateClient();
        }
    }
}
