using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Synapse.Factories.Interfaces
{
    public interface ILogger
    {
        public ILoggerFactory BuildLoggerFactory(IConfiguration configuration);
    }
}
