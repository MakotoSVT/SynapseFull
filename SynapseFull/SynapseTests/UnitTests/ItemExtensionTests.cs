using Synapse.Extensions;
using Synapse.Models;

namespace SynapseTests.UnitTests
{
    [TestFixture]
    public class ItemExtensionTests
    {
        [Test]
        public void IsItemDelivered_ReturnsTrue_WhenStatusIsDelivered()
        {
            // Arrange
            var item = new Item { Status = "Delivered" };

            // Act
            var result = item.IsItemDelivered();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsItemDelivered_ReturnsFalse_WhenStatusIsNotDelivered()
        {
            // Arrange
            var item = new Item { Status = "Pending" };

            // Act
            var result = item.IsItemDelivered();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IncrementDeliveryNotification_IncrementsNotificationCount()
        {
            // Arrange
            var item = new Item { DeliveryNotification = 0 };

            // Act
            var updatedItem = item.IncrementDeliveryNotification();

            // Assert
            Assert.AreEqual(1, updatedItem.DeliveryNotification);
        }
    }
}
