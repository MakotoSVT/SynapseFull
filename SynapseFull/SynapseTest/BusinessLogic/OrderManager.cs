using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Synapse.Extensions;
using Synapse.Models;
using Synapse.Providers;

namespace Synapse.BusinessLogic
{
    public class OrderManager : IOrderManager
    {
        public IAlertProvider _alertProvider { get; set; }
        public ILogger _logger { get; set; }

        public OrderManager(IConfiguration configuration, ILogger logger)
        {
            _alertProvider = new AlertProvider(configuration, logger);
            _logger = logger;
        }

        public Order ProcessOrder(Order order)
        {
            if (order != null)
            {
                var deliveredItems = order.Items.Where(x => x.IsItemDelivered()).ToList();

                if (deliveredItems.Any())
                {
                    foreach (var item in deliveredItems)
                    {
                        var itemIndex = deliveredItems.IndexOf(item);
                        // this should go in a method
                        var message = $"Alert for delivered item: Order {order.OrderId}, Item: {item.Description}, " +
                                  $"Delivery Notifications: {item.DeliveryNotification}";

                       var alertResult = _alertProvider.SendAlertMessage(message);

                        if (alertResult)
                        {
                            deliveredItems[itemIndex].IncrementDeliveryNotification();
                            _logger.LogInformation($"Alert sent for delivered item: {item.Description}");
                            continue;
                        }

                        _logger.LogError($"Failed to send alert for delivered item: {item.Description}");
                    }

                    deliveredItems.ForEach(x => x.IncrementDeliveryNotification());

                    var updatedOrderItems = deliveredItems;

                    var notDeliveredITems = order.Items.Where(x => !x.IsItemDelivered());

                    if (notDeliveredITems.Any())
                    {
                        deliveredItems.AddRange(notDeliveredITems);
                    }

                    order.Items = updatedOrderItems;
                }
            }

            return order;
        }
    }
}
