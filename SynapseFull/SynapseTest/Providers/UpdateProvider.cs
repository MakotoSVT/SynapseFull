using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Synapse.Models;

namespace Synapse.Providers
{
    public class UpdateProvider : BaseProvider, IUpdateProvider
    {
        public string _apiUrl { get; set; }

        public UpdateProvider(IConfiguration configuration, ILogger logger) : base(configuration, logger)
        {
            _apiUrl = Configuration["AppSettings:UpdateAPI"];
        }

        public async Task<int> SendAlertAndUpdateOrder(Order order)
        {
            try
            {
                if (order == null)
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        httpClient.Timeout = TimeSpan.FromSeconds(ApiTimeout);
                        string updateApiUrl = $"{_apiUrl}update";

                        var content = new StringContent(JObject.FromObject(order).ToString(), System.Text.Encoding.UTF8, "application/json");
                        var response = await httpClient.PostAsync(updateApiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            var sendResult = SendAlertMessage($"Updated order sent for processing: OrderId {order.OrderId}");

                            return 1;
                        }
                    }

                    Logger.LogError($"Failed to send updated order for processing: OrderId {order.OrderId}");
                    return 0;
                }

                Logger.LogError($"Failed to send updated order for processing: OrderId is null");
                return 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to send updated order for processing. ERROR: {ex.Message}");
            }

            return 0;
        }
    }
}
