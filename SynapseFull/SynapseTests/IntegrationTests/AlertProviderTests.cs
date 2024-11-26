using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Synapse.Providers;
using System.Net;

namespace SynapseTests.Integrations
{
    [TestFixture]
    public class AlertProviderTests
    {
        private Mock<IConfiguration> _configurationMock;
        private Mock<ILogger> _loggerMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private AlertProvider _alertProvider;
        private const string ApiUrl = "https://fakeapi.com/";

        [SetUp]
        public void SetUp()
        {
            // Mock IConfiguration
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["AppSettings:AlertAPI"]).Returns(ApiUrl);

            // Mock ILogger
            _loggerMock = new Mock<ILogger>();

            // Mock HttpMessageHandler
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            // Instantiate AlertProvider
            _alertProvider = new AlertProvider(_configurationMock.Object, _loggerMock.Object)
            {
                _alertApiUrl = ApiUrl
            };
        }

        // without active nad functional API's positive tests are not possible
        [Test]
        public void SendAlertMessage_ReturnsFalse_WhenApiCallFails()
        {
            // Arrange
            var message = "Test Alert Message";
            var alertApiUrl = $"{ApiUrl}alerts";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post && req.RequestUri.ToString() == alertApiUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            using var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            var result = false;

            // Act
            result = _alertProvider.SendAlertMessage(message);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SendAlertMessage_LogsError_WhenExceptionIsThrown()
        {
            // Arrange
            var message = "Test Alert Message";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            using var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            var result = false;

            // Act
            result = _alertProvider.SendAlertMessage(message);

            // Assert
            Assert.IsFalse(result);
            _loggerMock.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to send alert")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
