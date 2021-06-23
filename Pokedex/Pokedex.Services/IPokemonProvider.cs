using System.Threading.Tasks;
using Pokedex.Model;

namespace Pokedex.Services
{
    public interface IPokemonProvider
    {
        Task<Pokemon> GetPokemonAsync(string name);
    }
}