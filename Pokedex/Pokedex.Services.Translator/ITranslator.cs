using System.Threading.Tasks;

namespace Pokedex.Services.Translator
{
    public interface ITranslator
    {
        string Name { get; }
        Task<string> Translate(string value);
    }
}
