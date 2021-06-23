using System.Threading.Tasks;
using Pokedex.Model;

namespace Pokedex.Services
{
    public interface IPokemonService
    {
        Task<Pokemon> GetPokemonAsync(string name, bool withStandardDescription);
    }
}