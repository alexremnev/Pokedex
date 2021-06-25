using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Pokedex.Common.Exceptions;
using Xunit;

namespace Pokedex.Services.Translator.Tests
{
    public class YodaTranslatorTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactory;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        private const string ExpectedUrl = "http://yoda.test.com/";

        private const string Value = "Master Obiwan has lost a planet";

        public YodaTranslatorTests()
        {
            _httpClientFactory = new Mock<IHttpClientFactory>();

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
            var yodaTranslator = new YodaTranslator(_httpClientFactory.Object, ExpectedUrl);

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

            var yodaTranslator = new YodaTranslator(_httpClientFactory.Object, ExpectedUrl);

            // Act
            var translate = await yodaTranslator.Translate(Value);

            // Assert
            Assert.Null(translate);
        }

        [Fact]
        public async Task GivenYodaTranslator_WhenTranslateAsync_ThenReturnsServiceUnavailableException()
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

            var yodaTranslator = new YodaTranslator(_httpClientFactory.Object, ExpectedUrl);

            // Act
            var exception = await Assert.ThrowsAsync<ServiceUnavailableException>(() => yodaTranslator.Translate(Value));

            // Assert
            Assert.Equal($"Can not translate using Yoda translator, Error", exception.Message);
        }
    }
}
