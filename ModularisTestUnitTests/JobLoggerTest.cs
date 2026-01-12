using ModularisTest;
using ModularisTest.Configuration;
using ModularisTest.Destinations;
using ModularisTest.Enums;
using ModularisTest.Exceptions;
using ModularisTest.Extensions;
using ModularisTest.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace ModularisTestUnitTests
{
    [TestClass]
    public class JobLoggerTest
    {
        private const string TestMessage = "Test Message";
        private const string WarningMessage = "Test Warning";
        private const string ErrorMessage = "Test Error";
        private string _testLogDirectory;

        [TestInitialize]
        public void Initialize()
        {
            // Create a temporary directory for test logs
            _testLogDirectory = Path.Combine(Path.GetTempPath(), "JobLoggerTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testLogDirectory);

            // Reset the singleton instance before each test
            ResetJobLoggerInstance();

            // Set up dependency injection with test-specific file destination
            var services = new ServiceCollection();
            services.AddLogging();
            var serviceProvider = services.BuildServiceProvider();

            // Get configuration and enabled message types
            var configuration = serviceProvider.GetRequiredService<LogConfiguration>();
            var enabledMessageTypes = configuration.GetEnabledMessageTypes();

            // Create destinations manually for testing (with test directory)
            var destinations = new List<ILogDestination>
            {
                new FileLogDestination(_testLogDirectory),
                new ConsoleLogDestination()
            };

            // Initialize JobLogger singleton with dependencies
            JobLogger.Initialize(destinations, enabledMessageTypes);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Reset the singleton instance after each test
            ResetJobLoggerInstance();

            // Clean up test log directory
            if (Directory.Exists(_testLogDirectory))
            {
                try
                {
                    Directory.Delete(_testLogDirectory, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
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

        [TestMethod]
        public void JobLoggerFileDestinationTest()
        {
            // Arrange
            var testMessage = "File Log Test Message";
            var expectedFileName = "LogFile" + DateTime.Now.ToShortDateString().Replace("/", ".") + ".txt";
            var expectedFilePath = Path.Combine(_testLogDirectory, expectedFileName);

            // Act
            JobLogger.Instance.LogMessage(testMessage, LogMessageType.Message);

            // Assert
            Assert.IsTrue(File.Exists(expectedFilePath), "Log file should be created");
            
            var fileContent = File.ReadAllText(expectedFilePath);
            Assert.IsTrue(fileContent.Contains(testMessage), "File should contain the logged message");
            Assert.IsTrue(fileContent.Contains("Message"), "File should contain the message type label");
            Assert.IsTrue(fileContent.Contains(DateTime.Now.ToShortDateString()), "File should contain the date");
        }

        [TestMethod]
        public void JobLoggerFileDestinationMultipleMessagesTest()
        {
            // Arrange
            var message1 = "First Message";
            var message2 = "Second Message";
            var message3 = "Third Message";
            var expectedFileName = "LogFile" + DateTime.Now.ToShortDateString().Replace("/", ".") + ".txt";
            var expectedFilePath = Path.Combine(_testLogDirectory, expectedFileName);

            // Act
            JobLogger.Instance.LogMessage(message1, LogMessageType.Message);
            JobLogger.Instance.LogMessage(message2, LogMessageType.Warning);
            JobLogger.Instance.LogMessage(message3, LogMessageType.Error);

            // Assert
            Assert.IsTrue(File.Exists(expectedFilePath), "Log file should be created");
            
            var fileContent = File.ReadAllText(expectedFilePath);
            Assert.IsTrue(fileContent.Contains(message1), "File should contain the first message");
            Assert.IsTrue(fileContent.Contains(message2), "File should contain the second message");
            Assert.IsTrue(fileContent.Contains(message3), "File should contain the third message");
            Assert.IsTrue(fileContent.Contains("Message"), "File should contain Message type");
            Assert.IsTrue(fileContent.Contains("Warning"), "File should contain Warning type");
            Assert.IsTrue(fileContent.Contains("Error"), "File should contain Error type");
        }

        [TestMethod]
        public void JobLoggerFileDestinationAppendsMessagesTest()
        {
            // Arrange
            var message1 = "First Log Entry";
            var message2 = "Second Log Entry";
            var expectedFileName = "LogFile" + DateTime.Now.ToShortDateString().Replace("/", ".") + ".txt";
            var expectedFilePath = Path.Combine(_testLogDirectory, expectedFileName);

            // Act
            JobLogger.Instance.LogMessage(message1, LogMessageType.Message);
            var firstWriteContent = File.ReadAllText(expectedFilePath);
            
            JobLogger.Instance.LogMessage(message2, LogMessageType.Message);
            var secondWriteContent = File.ReadAllText(expectedFilePath);

            // Assert
            Assert.IsTrue(secondWriteContent.Contains(message1), "File should still contain the first message after second write");
            Assert.IsTrue(secondWriteContent.Contains(message2), "File should contain the second message");
            Assert.IsTrue(secondWriteContent.Length > firstWriteContent.Length, "File should grow with new entries");
        }

        [TestMethod]
        public void JobLoggerConsoleDestinationTest()
        {
            // Arrange
            var testMessage = "Console Log Test Message";
            var originalOut = Console.Out;
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            try
            {
                // Act
                JobLogger.Instance.LogMessage(testMessage, LogMessageType.Message);

                // Assert
                var output = stringWriter.ToString();
                Assert.IsTrue(output.Contains(testMessage), "Console output should contain the logged message");
                Assert.IsTrue(output.Contains(DateTime.Now.ToShortDateString()), "Console output should contain the date");
            }
            finally
            {
                // Restore original console output
                Console.SetOut(originalOut);
                stringWriter.Dispose();
            }
        }

        [TestMethod]
        public void JobLoggerConsoleDestinationAllMessageTypesTest()
        {
            // Arrange
            var message = "Test Message";
            var warning = "Test Warning";
            var error = "Test Error";
            var originalOut = Console.Out;
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            try
            {
                // Act
                JobLogger.Instance.LogMessage(message, LogMessageType.Message);
                JobLogger.Instance.LogMessage(warning, LogMessageType.Warning);
                JobLogger.Instance.LogMessage(error, LogMessageType.Error);

                // Assert
                var output = stringWriter.ToString();
                Assert.IsTrue(output.Contains(message), "Console output should contain the message");
                Assert.IsTrue(output.Contains(warning), "Console output should contain the warning");
                Assert.IsTrue(output.Contains(error), "Console output should contain the error");
            }
            finally
            {
                // Restore original console output
                Console.SetOut(originalOut);
                stringWriter.Dispose();
            }
        }

        [TestMethod]
        public void JobLoggerBothDestinationsTest()
        {
            // Arrange
            var testMessage = "Both Destinations Test";
            var expectedFileName = "LogFile" + DateTime.Now.ToShortDateString().Replace("/", ".") + ".txt";
            var expectedFilePath = Path.Combine(_testLogDirectory, expectedFileName);
            var originalOut = Console.Out;
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            try
            {
                // Act
                JobLogger.Instance.LogMessage(testMessage, LogMessageType.Message);

                // Assert - File destination
                Assert.IsTrue(File.Exists(expectedFilePath), "Log file should be created");
                var fileContent = File.ReadAllText(expectedFilePath);
                Assert.IsTrue(fileContent.Contains(testMessage), "File should contain the logged message");

                // Assert - Console destination
                var consoleOutput = stringWriter.ToString();
                Assert.IsTrue(consoleOutput.Contains(testMessage), "Console output should contain the logged message");
            }
            finally
            {
                // Restore original console output
                Console.SetOut(originalOut);
                stringWriter.Dispose();
            }
        }

        [TestMethod]
        public void JobLoggerFileDestinationCreatesDirectoryTest()
        {
            // Arrange
            var nonExistentDirectory = Path.Combine(_testLogDirectory, "SubDirectory", "Nested");
            var expectedFileName = "LogFile" + DateTime.Now.ToShortDateString().Replace("/", ".") + ".txt";
            var expectedFilePath = Path.Combine(nonExistentDirectory, expectedFileName);

            // Reset and reinitialize with new directory
            ResetJobLoggerInstance();
            var services = new ServiceCollection();
            services.AddLogging();
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<LogConfiguration>();
            var enabledMessageTypes = configuration.GetEnabledMessageTypes();
            var destinations = new List<ILogDestination>
            {
                new FileLogDestination(nonExistentDirectory),
                new ConsoleLogDestination()
            };
            JobLogger.Initialize(destinations, enabledMessageTypes);

            // Act
            JobLogger.Instance.LogMessage("Directory Test", LogMessageType.Message);

            // Assert
            Assert.IsTrue(Directory.Exists(nonExistentDirectory), "Directory should be created automatically");
            Assert.IsTrue(File.Exists(expectedFilePath), "Log file should be created in the new directory");
        }

        private void ResetJobLoggerInstance()
        {
            var field = typeof(JobLogger).GetField("_instance", 
                BindingFlags.NonPublic | BindingFlags.Static);
            field?.SetValue(null, null);
        }
    }
}
