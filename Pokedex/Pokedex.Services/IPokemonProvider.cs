using System.Threading.Tasks;
using Pokedex.Services.Models;

namespace Pokedex.Services
{
    public interface IPokemonProvider
    {
        Task<PokemonDetails> GetPokemonAsync(string name);
    }
}