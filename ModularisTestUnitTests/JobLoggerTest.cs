using ModularisTest;
using ModularisTest.Configuration;
using ModularisTest.Enums;
using ModularisTest.Exceptions;
using ModularisTest.Extensions;
using ModularisTest.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ModularisTestUnitTests
{
    [TestClass]
    public class JobLoggerTest
    {
        private const string TestMessage = "Test Message";
        private const string WarningMessage = "Test Warning";
        private const string ErrorMessage = "Test Error";

        [TestInitialize]
        public void Initialize()
        {
            // Reset the singleton instance before each test
            ResetJobLoggerInstance();

            // Set up dependency injection
            var services = new ServiceCollection();
            services.AddLogging();  // This is where AddLogging is called!
            var serviceProvider = services.BuildServiceProvider();

            // Get destinations and enabled message types from configuration
            var configuration = serviceProvider.GetRequiredService<LogConfiguration>();
            var destinations = serviceProvider.GetRequiredService<IEnumerable<ILogDestination>>();
            var enabledMessageTypes = configuration.GetEnabledMessageTypes();

            // Initialize JobLogger singleton with dependencies
            JobLogger.Initialize(destinations, enabledMessageTypes);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Reset the singleton instance after each test
            ResetJobLoggerInstance();
        }

        [TestMethod]
        public void JobLoggerBasicTest()
        {
            JobLogger.Instance.LogMessage(TestMessage, LogMessageType.Message);
            JobLogger.Instance.LogMessage(WarningMessage, LogMessageType.Warning);
            JobLogger.Instance.LogMessage(ErrorMessage, LogMessageType.Error);
        }

        [TestMethod]
        public void JobLoggerMessageTypeTest()
        {
            JobLogger.Instance.LogMessage(TestMessage, LogMessageType.Message);
            JobLogger.Instance.LogMessage(WarningMessage, LogMessageType.Warning);
            JobLogger.Instance.LogMessage(ErrorMessage, LogMessageType.Error);
        }

        [TestMethod]
        [ExpectedException(typeof(JobLoggerNotInitializedException))]
        public void JobLoggerNotInitializedTest()
        {
            // Reset the singleton for this test
            ResetJobLoggerInstance();

            // This should throw an exception
            var instance = JobLogger.Instance;
        }

        [TestMethod]
        public void JobLoggerEmptyMessageTest()
        {
            // Empty messages should be ignored (no exception thrown)
            JobLogger.Instance.LogMessage("", LogMessageType.Message);
            JobLogger.Instance.LogMessage("   ", LogMessageType.Message);
            JobLogger.Instance.LogMessage(null, LogMessageType.Message);
        }

        private void ResetJobLoggerInstance()
        {
            var field = typeof(JobLogger).GetField("_instance", 
                BindingFlags.NonPublic | BindingFlags.Static);
            field?.SetValue(null, null);
        }
    }
}
