using Synapse.Models;

namespace Synapse.Extensions
{
    public static class ItemExtensions
    {
        public static bool IsItemDelivered(this Item orderItem)
        {
            return orderItem.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase);
        }

        public static Item IncrementDeliveryNotification(this Item item)
        {
            item.DeliveryNotification = item.DeliveryNotification + 1;

            return item;
        }
    }
}
