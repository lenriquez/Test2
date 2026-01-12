using Microsoft.Extensions.DependencyInjection;
using ModularisTest.Configuration;
using ModularisTest.Factories;
using ModularisTest.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ModularisTest.Extensions
{
    public static class LoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddLogging(this IServiceCollection services)
        {
            // Register configuration
            services.AddSingleton<LogConfiguration>();

            // Register factory
            services.AddSingleton<LogDestinationFactory>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<LogConfiguration>();
                return new LogDestinationFactory(configuration);
            });

            // Register destinations
            services.AddSingleton<IEnumerable<ILogDestination>>(serviceProvider =>
            {
                var factory = serviceProvider.GetRequiredService<LogDestinationFactory>();
                return factory.CreateDestinations().ToList();
            });

            return services;
        }
    }
}
