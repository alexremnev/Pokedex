using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pokedex.Services;

namespace Pokedex.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonService _pokemonService;

        public PokemonController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            return await GetPokemonAsync(name, true);
        }

        [HttpGet("translated/{name}")]
        public async Task<IActionResult> GetTranslated(string name)
        {
            return await GetPokemonAsync(name, false);
        }

        private async Task<IActionResult> GetPokemonAsync(string name, bool withStandardDescription)
        {
            if (!VerifyName(name))
            {
                return BadRequest("Only letters allowed");
            }

            var pokemon = await _pokemonService.GetPokemonAsync(name, withStandardDescription);

            if (pokemon == null)
            {
                return NotFound();
            }

            return Ok(pokemon);
        }

        private static bool VerifyName(string name)
        {
            //only letters allowed
            return Regex.IsMatch(name, @"^[a-zA-Z]+$");
        }
    }
}
