using ModularisTest.Configuration;
using ModularisTest.Destinations;
using ModularisTest.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ModularisTest.Factories
{
    public class LogDestinationFactory
    {
        private readonly LogConfiguration _configuration;

        public LogDestinationFactory(LogConfiguration configuration)
        {
            _configuration = configuration ?? throw new System.ArgumentNullException(nameof(configuration));
        }

        public IEnumerable<ILogDestination> CreateDestinations()
        {
            var destinations = new List<ILogDestination>();
            var enabledDestinations = _configuration.GetEnabledDestinations();

            foreach (var destinationName in enabledDestinations)
            {
                var destination = CreateDestination(destinationName);
                if (destination != null)
                {
                    destinations.Add(destination);
                }
            }

            return destinations;
        }

        private ILogDestination CreateDestination(string destinationName)
        {
            switch (destinationName.ToLowerInvariant())
            {
                case "file":
                    var logFileDirectory = _configuration.GetFileSetting("LogFileDirectory");
                    return new FileLogDestination(logFileDirectory);

                case "console":
                    return new ConsoleLogDestination();

                case "database":
                    var connectionString = _configuration.GetDestinationConnectionString("Database");
                    if (string.IsNullOrWhiteSpace(connectionString))
                    {
                        throw new System.Configuration.ConfigurationErrorsException(
                            "Database destination requires a connectionString attribute in the configuration.");
                    }
                    return new DatabaseLogDestination(connectionString);

                default:
                    return null;
            }
        }
    }
}
