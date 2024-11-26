using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Synapse.Providers
{
    public class BaseProvider
    {
        public IConfiguration Configuration;
        public ILogger Logger { get; set; }
        public int ApiTimeout { get; set; }
        public string _alertApiUrl { get; set; }


        public BaseProvider(IConfiguration configuration, ILogger logger)
        {
            Configuration = configuration;
            ApiTimeout = Convert.ToInt32(configuration["AppSettings:ApiTimeout"]);
            Logger = logger;
        }

        public bool SendAlertMessage(string message)
        {
            var result = false;

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(ApiTimeout);

                    var alertApiUrl = $"{_alertApiUrl}alerts";

                    var alertData = new
                    {
                        Message = message
                    };

                    var content = new StringContent(JObject.FromObject(alertData).ToString(), System.Text.Encoding.UTF8, "application/json");
                    var response = httpClient.PostAsync(alertApiUrl, content).Result;

                    result = response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to send alert for delivered item. ERROR: {ex.Message}");
            }

            return result;
        }
    }
}
