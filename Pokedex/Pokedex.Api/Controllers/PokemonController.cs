using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Pokedex.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly ILogger<PokemonController> _logger;

        public PokemonController(ILogger<PokemonController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public  IActionResult Get()
        {
            return Ok();
        }
    }
}
