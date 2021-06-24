using System.Collections.Generic;
using Moq;
using Pokedex.Services.Translator;
using Xunit;

namespace Pokedex.Services.Tests
{
    public class TranslatorFactoryTests
    {
        private readonly IEnumerable<ITranslator> _translators;

        public TranslatorFactoryTests()
        {
            var shakespeareTranslator = new Mock<ITranslator>();
            shakespeareTranslator.Setup(p => p.Name).Returns(TranslatorNames.ShakespeareTranslatorName);
            var yodaTranslator = new Mock<ITranslator>();
            yodaTranslator.Setup(p => p.Name).Returns(TranslatorNames.YodaTranslatorName);

            _translators = new List<ITranslator>()
            {
                shakespeareTranslator.Object, yodaTranslator.Object
            };
        }

        [Fact]
        public void GivenShakespeareTranslatorName_WhenCreateTranslator_ThenReturnsShakespeareTranslator()
        {
            // Arrange
            var translatorFactory = new TranslatorFactory(_translators);

            // Act
            var translator = translatorFactory.Create(TranslatorNames.ShakespeareTranslatorName);

            // Assert
            Assert.Equal(TranslatorNames.ShakespeareTranslatorName, translator.Name);
        }

        [Fact]
        public void GivenYodaTranslatorName_WhenCreateTranslator_ThenReturnsYodaTranslator()
        {
            // Arrange
            var translatorFactory = new TranslatorFactory(_translators);

            // Act
            var translator = translatorFactory.Create(TranslatorNames.YodaTranslatorName);

            // Assert
            Assert.Equal(TranslatorNames.YodaTranslatorName, translator.Name);
        }

        [Fact]
        public void GivenEmptyName_WhenCreateTranslator_ThenReturnsDefaultTranslator()
        {
            // Arrange
            var translatorFactory = new TranslatorFactory(_translators);

            // Act
            var translator = translatorFactory.Create("");

            // Assert
            Assert.Null(translator);
        }
    }
}
