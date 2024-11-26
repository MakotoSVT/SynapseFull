using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Synapse.Providers;
using System.Net;

namespace SynapseTests.Integrations
{
    [TestFixture]
    public class OrderProviderTests
    {
        private Mock<IConfiguration> _configurationMock;
        private Mock<ILogger> _loggerMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private OrderProvider _orderProvider;
        private string _apiUrl = "https://fakeapi.com/";

        [SetUp]
        public void SetUp()
        {
            // Mock IConfiguration
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["AppSettings:OrderAPI"]).Returns(_apiUrl);

            // Mock ILogger
            _loggerMock = new Mock<ILogger>();

            // Mock HttpMessageHandler
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            // Use the mocked handler with HttpClient
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            // Instantiate the OrderProvider with dependencies
            _orderProvider = new OrderProvider(_configurationMock.Object, _loggerMock.Object);
        }

        // without active nad functional API's positive tests are not possible
        [Test]
        public async Task FetchMedicalEquipmentOrders_ReturnsNull_WhenApiCallFails()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            // Act
            var orders = await _orderProvider.FetchMedicalEquipmentOrders();

            // Assert
            Assert.IsNull(orders);
        }

        [Test]
        public async Task FetchMedicalEquipmentOrders_LogsError_WhenExceptionIsThrown()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var orders = await _orderProvider.FetchMedicalEquipmentOrders();

            // Assert
            Assert.IsNull(orders);
            _loggerMock.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to fetch orders")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
