namespace Pokedex.Services.Translator
{
    public interface ITranslatorFactory
    {
        ITranslator Create(string name);
    }
}