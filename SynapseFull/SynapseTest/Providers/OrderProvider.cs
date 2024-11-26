using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Synapse.Models;

namespace Synapse.Providers
{
    public class OrderProvider : BaseProvider, IOrderProvider
    {
        public string _apiUrl { get; set; }

        public OrderProvider(IConfiguration configuration, ILogger logger) : base(configuration, logger)
        {
            _apiUrl = Configuration["AppSettings:OrderAPI"];
        }

        public async Task<IEnumerable<Order>> FetchMedicalEquipmentOrders()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(ApiTimeout);
                    var ordersApiUrl = $"{_apiUrl}orders";
                    var response = await httpClient.GetAsync(ordersApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var ordersData = await response.Content.ReadAsStringAsync();

                        var orders = JsonConvert.DeserializeObject<IEnumerable<Order>>(ordersData);

                        return orders;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to fetch orders from Order API. ERROR: {ex.Message}");
            }

            return null;
        }
    }
}
