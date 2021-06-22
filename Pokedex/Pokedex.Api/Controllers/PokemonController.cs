using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pokedex.Services;

namespace Pokedex.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonProvider _pokemonProvider;
        private readonly ILogger<PokemonController> _logger;

        public PokemonController(IPokemonProvider pokemonProvider, ILogger<PokemonController> logger)
        {
            _pokemonProvider = pokemonProvider;
            _logger = logger;
        }

        [HttpGet("translated/{name}")]
        public async Task<IActionResult> Get(string name)
        {
            //validate and return if need BadRequest
            var pokemon = await _pokemonProvider.GetPokemonAsync(name);

            //map to Pokemon result

            return Ok(pokemon);
        }
    }
}
