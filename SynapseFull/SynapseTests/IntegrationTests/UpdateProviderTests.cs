using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Synapse.Models;
using Synapse.Providers;
using System.Net;

namespace SynapseTests.Integrations
{
    [TestFixture]
    public class UpdateProviderTests
    {
        private Mock<IConfiguration> _configurationMock;
        private Mock<ILogger> _loggerMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private UpdateProvider _updateProvider;
        private const string ApiUrl = "https://fakeapi.com/";

        [SetUp]
        public void SetUp()
        {
            // Mock IConfiguration
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["AppSettings:UpdateAPI"]).Returns(ApiUrl);

            // Mock ILogger
            _loggerMock = new Mock<ILogger>();

            // Mock HttpMessageHandler
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            // Instantiate UpdateProvider
            _updateProvider = new UpdateProvider(_configurationMock.Object, _loggerMock.Object);
        }

        // without active nad functional API's positive tests are not possible
        [Test]
        public async Task SendAlertAndUpdateOrder_ReturnsFalse_WhenApiCallFails()
        {
            // Arrange
            var fakeOrder = new Order { OrderId = "1337", Status = "Delivered" };
            var updateApiUrl = $"{ApiUrl}update";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post && req.RequestUri.ToString() == updateApiUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            // Act
            var result = await _updateProvider.SendAlertAndUpdateOrder(fakeOrder);

            // Assert
            Assert.IsTrue(result == 0);
        }

        [Test]
        public async Task SendAlertAndUpdateOrder_LogsError_WhenExceptionIsThrown()
        {
            // Arrange
            var fakeOrder = new Order { OrderId = "1337", Status = "Delivered" };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await _updateProvider.SendAlertAndUpdateOrder(fakeOrder);

            // Assert
            Assert.IsTrue(result == 0);

            _loggerMock.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to send updated order")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
