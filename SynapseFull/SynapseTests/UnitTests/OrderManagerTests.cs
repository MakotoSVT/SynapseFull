using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Synapse.BusinessLogic;
using Synapse.Models;
using Synapse.Providers;

namespace SynapseTests.UnitTests
{
    [TestFixture]
    public class OrderManagerTests
    {
        private Mock<IAlertProvider> _mockAlertProvider;
        private Mock<ILogger> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;
        private OrderManager _orderManager;

        [SetUp]
        public void SetUp()
        {
            // Mock dependencies
            _mockAlertProvider = new Mock<IAlertProvider>();
            _mockLogger = new Mock<ILogger>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Inject mocks into the OrderManager
            _orderManager = new OrderManager(_mockConfiguration.Object, _mockLogger.Object)
            {
                _alertProvider = _mockAlertProvider.Object // Use the mocked IAlertProvider
            };
        }

        [Test]
        public void ProcessOrder_ShouldSendAlertForDeliveredItems()
        {
            // Arrange
            var order = new Order
            {
                OrderId = "1337",
                Items = new List<Item>
                {
                    new Item
                    {
                        Description = "Item 1",
                        DeliveryNotification = 0,
                        Status = "Delivered"
                    },
                    new Item
                    {
                        Description = "Item 3",
                        DeliveryNotification = 0,
                        Status = "Delivered"
                    },
                    new Item
                    {
                        Description = "Item 2",
                        DeliveryNotification = 0,
                        Status = ""
                    }
                }
            };

            // Configure the mock to simulate successful alert message sending
            _mockAlertProvider
                .Setup(p => p.SendAlertMessage(It.IsAny<string>()))
                .Returns(true);

            // Act
            var processedOrder = _orderManager.ProcessOrder(order);

            // Assert
            _mockAlertProvider.Verify(p => p.SendAlertMessage(It.Is<string>(
                message => message.Contains("Item 1"))), Times.Once);

            var testthing = processedOrder.Items;

            Assert.AreEqual(2, testthing.First().DeliveryNotification);
        }

        [Test]
        public void ProcessOrder_ShouldHandleNullOrderGracefully()
        {
            // Act
            var processedOrder = _orderManager.ProcessOrder(null);

            // Assert
            Assert.IsNull(processedOrder);
        }

        [Test]
        public void ProcessOrder_ShouldNotSendAlertsIfNoItemsAreDelivered()
        {
            // Arrange
            var order = new Order
            {
                OrderId = "1338",
                Items = new List<Item>
                {
                    new Item
                    {
                        Description = "Item 1",
                        DeliveryNotification = 0,
                        Status = ""
                    }
                }
            };

            // Act
            var processedOrder = _orderManager.ProcessOrder(order);

            // Assert
            _mockAlertProvider.Verify(p => p.SendAlertMessage(It.IsAny<string>()), Times.Never);
            Assert.AreEqual(0, processedOrder.Items.First().DeliveryNotification);
        }
    }
}
