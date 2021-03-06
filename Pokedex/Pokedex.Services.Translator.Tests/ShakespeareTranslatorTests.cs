using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Pokedex.Services.Translator.Tests
{
    public class ShakespeareTranslatorTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactory;
        private readonly Mock<ILoggerFactory> _loggerFactory;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        private const string ExpectedUrl = "http://shakespeare.test.com/";

        private readonly string value = "You gave Mr. Tim a hearty meal, but unfortunately what he ate made him die.";

        public ShakespeareTranslatorTests()
        {
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();

            _loggerFactory.Setup(p => p.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == HttpMethod.Post
                        && r.RequestUri.ToString().Equals(ExpectedUrl)
                        && r.Content.ReadAsStringAsync().Result == SerializeObject(value)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        "{'success': {'total': 1 },'contents': { 'translated': 'Thee did giveth mr. Tim a hearty meal, but unfortunately what he did doth englut did maketh him kicketh the bucket.','text': 'You gave Mr. Tim a hearty meal, but unfortunately what he ate made him die.','translation': 'shakespeare'}}"
                    )
                });
            var client = new HttpClient(_mockHttpMessageHandler.Object);
            _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        }

        private static string SerializeObject(string text)
        {
            return JsonConvert.SerializeObject(new { text });
        }

        [Fact]
        public async Task GivenShakespeareTranslator_WhenTranslateAsync_ThenReturnsTranslation()
        {
            // Arrange
            var shakespeareTranslator = new ShakespeareTranslator(_httpClientFactory.Object, _loggerFactory.Object, ExpectedUrl);

            // Act
            var translate = await shakespeareTranslator.Translate(value);

            // Assert
            Assert.Equal(
                "Thee did giveth mr. Tim a hearty meal, but unfortunately what he did doth englut did maketh him kicketh the bucket.",
                translate);
        }

        [Fact]
        public async Task GivenShakespeareTranslator_WhenTranslateAsync_ThenReturnsEmptyTranslation()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == HttpMethod.Post
                        && r.RequestUri.ToString().Equals(ExpectedUrl)
                        && r.Content.ReadAsStringAsync().Result == SerializeObject(value)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        "{'success': {'total': 0 },'contents': {}}"
                    )
                });

            var shakespeareTranslator = new ShakespeareTranslator(_httpClientFactory.Object, _loggerFactory.Object, ExpectedUrl);

            // Act
            var translate = await shakespeareTranslator.Translate(value);

            // Assert
            Assert.Null(translate);
        }

        [Fact]
        public async Task GivenShakespeareTranslator_WhenTranslateAsync_ThenReturnsStandardDescription()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == HttpMethod.Post
                        && r.RequestUri.ToString().Equals(ExpectedUrl)
                        && r.Content.ReadAsStringAsync().Result == SerializeObject(value)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = "Error"
                });

            var shakespeareTranslator = new ShakespeareTranslator(_httpClientFactory.Object, _loggerFactory.Object, ExpectedUrl);

            // Act
            var actual = await shakespeareTranslator.Translate(value);

            // Assert
            Assert.Equal(value, actual);
        }
    }
}
