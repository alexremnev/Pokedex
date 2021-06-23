using System.Threading.Tasks;
using Pokedex.Common;

namespace Pokedex.Services
{
    public interface IPokemonProvider
    {
        Task<Pokemon> GetPokemonAsync(string name);
    }
}