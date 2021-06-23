using System.Collections.Generic;
using System.Linq;

namespace Pokedex.Services.Translator
{
    public class TranslatorFactory : ITranslatorFactory
    {
        private readonly IEnumerable<ITranslator> _translators;

        public TranslatorFactory(IEnumerable<ITranslator> translators)
        {
            _translators = translators;
        }
        public ITranslator Create(string name)
        {
            return _translators.FirstOrDefault(translator => translator.Name == name);
        }
    }
}