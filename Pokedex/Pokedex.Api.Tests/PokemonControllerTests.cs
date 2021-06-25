using System.Net;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.XUnit2;
using LightBDD.Framework.Scenarios;

namespace Pokedex.Api.Tests
{
    [FeatureDescription("Pokemon API")]
    public class PokemonControllerTests : FeatureFixture
    {
        [Scenario(DisplayName = "Get. If pokemon is requested then pokemon with description is returned")]
        public async Task If_pokemon_is_requested_then_pokemon_with_description_is_returned()
        {
            await Runner.WithContext<Context>()
                .AddSteps(
                    _ => _.Given_a_valid_pokemon_name()
                )
                .AddAsyncSteps(
                    _ => _.When_pokemon_is_requested()
                )
                .AddSteps(
                    _ => _.Then_return_proper_response(HttpStatusCode.OK)
                )
                .AddAsyncSteps(
                    _ => _.Then_return_proper_model("urban", false)
                        )
                .RunAsync();
        }

        [Scenario(DisplayName = "Get. If legendary pokemon is requested then pokemon with description is returned")]
        public async Task If_legendary_pokemon_is_requested_then_pokemon_with_description_is_returned()
        {
            await Runner.WithContext<Context>()
                .AddSteps(
                    _ => _.Given_a_legendary_pokemon_name()
                )
                .AddAsyncSteps(
                    _ => _.When_translated_pokemon_is_requested()
                )
                .AddSteps(
                    _ => _.Then_return_proper_response(HttpStatusCode.OK)
                )
                .AddAsyncSteps(
                    _ => _.Then_return_proper_model(null, true)
                )
                .RunAsync();
        }

        [Scenario(DisplayName = "Get. If invalid pokemon is requested then NotFound returned")]
        public async Task If_pokemon_is_requested_then_NotFound_returned()
        {
            await Runner.WithContext<Context>()
                .AddSteps(
                    _ => _.Given_a_invalid_pokemon_name()
                )
                .AddAsyncSteps(
                    _ => _.When_pokemon_is_requested()
                )
                .AddSteps(
                    _ => _.Then_return_proper_response(HttpStatusCode.NotFound)
                )
                .RunAsync();
        }
    }
}
