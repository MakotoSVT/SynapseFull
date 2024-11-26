using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Synapse.BusinessLogic;
using Synapse.Providers;

namespace Synapse.OrdersExample
{
    /// <summary>
    /// I Get a list of orders from the API
    /// I check if the order is in a delivered state, If yes then send a delivery alert and add one to deliveryNotification
    /// I then update the order.   
    /// </summary>
    public class Program
    {
        static IOrderProvider _orderProvider { get; set; }
        static IAlertProvider _alertProvider { get; set; }
        static IUpdateProvider _updateProvider { get; set; }
        static IOrderManager _orderManager { get; set; }
        static IConfiguration _configuration { get; set; }
        static Factories.Interfaces.ILogger _loggerFactory { get; set; }
        static ILogger<Program> _logger { get; set; }

        static void ConfigureStartup()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
            _configuration = builder.Build();

            _loggerFactory = new Factories.Logger(_configuration);

            var loggerFactory = _loggerFactory.BuildLoggerFactory(_configuration);
            _logger = loggerFactory.CreateLogger<Program>();

            _orderProvider = new OrderProvider(_configuration, _logger);
            _alertProvider = new AlertProvider(_configuration, _logger);
            _updateProvider = new UpdateProvider(_configuration, _logger);
        }

        static async Task<int> Main(string[] args)
        {
            var result = 0;

            try
            {
                ConfigureStartup();

                _logger.LogInformation("Start of App");

                var medicalEquipmentOrders = await _orderProvider.FetchMedicalEquipmentOrders();

                if (medicalEquipmentOrders != null)
                {
                    foreach (var order in medicalEquipmentOrders)
                    {
                        // check and update delivered orders
                        var updatedOrder = _orderManager.ProcessOrder(order);

                        if (updatedOrder != null)
                        {
                            // update order
                           result = result + _updateProvider.SendAlertAndUpdateOrder(updatedOrder).GetAwaiter().GetResult();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            _logger.LogInformation($"{result} results sent to relevant APIs.");

            return result;
        }
    }
}