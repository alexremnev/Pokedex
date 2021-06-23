using System.Threading.Tasks;
using Pokedex.Common;

namespace Pokedex.Services
{
    public interface IPokemonService
    {
        Task<Pokemon> GetPokemonAsync(string name, bool withStandardDescription);
    }
}