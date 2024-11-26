using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Synapse.Providers
{
    // This exists sot hat we can use the interface to access the base provider which houses the Alert functionality that other providers may use.
    public class AlertProvider : BaseProvider, IAlertProvider
    {
        public AlertProvider(IConfiguration configuration, ILogger logger) : base(configuration, logger)
        {

        }
    }
}
