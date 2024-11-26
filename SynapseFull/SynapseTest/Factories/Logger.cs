using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Synapse.Factories
{
    public class Logger : Interfaces.ILogger
    {
        public IConfiguration _configuration;

        public Logger(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ILoggerFactory BuildLoggerFactory(IConfiguration configuration)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Synapse.Program", LogLevel.Debug)
                    .AddConsole();

                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File(_configuration["AppSettings:LogPath"])
                    .CreateLogger();

                builder.AddSerilog();
            });

            return loggerFactory;
        }
    }
}
