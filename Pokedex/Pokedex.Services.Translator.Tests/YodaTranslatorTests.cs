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
    public class YodaTranslatorTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactory;
        private readonly Mock<ILoggerFactory> _loggerFactory;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        private const string ExpectedUrl = "http://yoda.test.com/";

        private const string Value = "Master Obiwan has lost a planet";

        public YodaTranslatorTests()
        {
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();

            _loggerFactory.Setup(p => p.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == HttpMethod.Post
                        && r.RequestUri.ToString().Equals(ExpectedUrl)
                        && r.Content.ReadAsStringAsync().Result == SerializeObject(Value)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        "{'success': {'total': 1 },'contents': { 'translated': 'Lost a planet, master obiwan has.','text': 'Master Obiwan has lost a planet','translation': 'yoda'}}"
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
        public async Task GivenYodaTranslator_WhenTranslateAsync_ThenReturnsTranslation()
        {
            // Arrange
            var yodaTranslator = new YodaTranslator(_httpClientFactory.Object, _loggerFactory.Object, ExpectedUrl);

            // Act
            var translate = await yodaTranslator.Translate(Value);

            // Assert
            Assert.Equal("Lost a planet, master obiwan has.", translate);
        }

        [Fact]
        public async Task GivenYodaTranslator_WhenTranslateAsync_ThenReturnsEmptyTranslation()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == HttpMethod.Post
                        && r.RequestUri.ToString().Equals(ExpectedUrl)
                        && r.Content.ReadAsStringAsync().Result == SerializeObject(Value)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        "{'success': {'total': 0 },'contents': {}}"
                    )
                });

            var yodaTranslator = new YodaTranslator(_httpClientFactory.Object, _loggerFactory.Object, ExpectedUrl);

            // Act
            var translate = await yodaTranslator.Translate(Value);

            // Assert
            Assert.Null(translate);
        }

        [Fact]
        public async Task GivenYodaTranslator_WhenTranslateAsync_ThenReturnsStandardDescription()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == HttpMethod.Post
                        && r.RequestUri.ToString().Equals(ExpectedUrl)
                        && r.Content.ReadAsStringAsync().Result == SerializeObject(Value)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = "Error"
                });

            var yodaTranslator = new YodaTranslator(_httpClientFactory.Object, _loggerFactory.Object, ExpectedUrl);

            // Act
            var actual = await yodaTranslator.Translate(Value);

            // Assert
            Assert.Equal(Value, actual);
        }
    }
}
