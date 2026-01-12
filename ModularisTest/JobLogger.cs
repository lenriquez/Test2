using ModularisTest.Destinations;
using ModularisTest.Enums;
using ModularisTest.Exceptions;
using ModularisTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModularisTest
{
    public class JobLogger
    {
        private static readonly object _lock = new object();
        private static JobLogger _instance;

        private readonly IEnumerable<ILogDestination> _destinations;
        private readonly HashSet<LogMessageType> _enabledMessageTypes;
        private bool _initialized;

        private JobLogger(IEnumerable<ILogDestination> destinations, IEnumerable<LogMessageType> enabledMessageTypes)
        {
            if (destinations == null || !destinations.Any())
            {
                throw new ArgumentException("At least one logging destination must be configured.", nameof(destinations));
            }

            if (enabledMessageTypes == null || !enabledMessageTypes.Any())
            {
                throw new ArgumentException("At least one message type must be enabled.", nameof(enabledMessageTypes));
            }

            _destinations = destinations;
            _enabledMessageTypes = new HashSet<LogMessageType>(enabledMessageTypes);
            _initialized = true;
        }

        public static JobLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new JobLoggerNotInitializedException();
                }
                return _instance;
            }
        }

        public static void Initialize(IEnumerable<ILogDestination> destinations, IEnumerable<LogMessageType> enabledMessageTypes)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("JobLogger has already been initialized.");
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new JobLogger(destinations, enabledMessageTypes);
                }
            }
        }

        public void LogMessage(string message, LogMessageType messageType)
        {
            if (!_initialized)
            {
                throw new JobLoggerNotInitializedException();
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var trimmedMessage = message.Trim();
            if (string.IsNullOrEmpty(trimmedMessage))
            {
                return;
            }

            if (!_enabledMessageTypes.Contains(messageType))
            {
                throw new ArgumentException($"Message type '{messageType}' is not enabled in the configuration.", nameof(messageType));
            }

            foreach (var destination in _destinations)
            {
                try
                {
                    destination.LogMessage(trimmedMessage, messageType);
                }
                catch (Exception ex)
                {
                    // Log error to console if available, but don't fail the entire logging operation
                    var consoleDestination = _destinations.OfType<ConsoleLogDestination>().FirstOrDefault();
                    if (consoleDestination != null)
                    {
                        try
                        {
                            consoleDestination.LogMessage($"Failed to log to destination: {ex.Message}", LogMessageType.Error);
                        }
                        catch
                        {
                            // If even console logging fails, we can't do anything
                        }
                    }
                }
            }
        }
    }
}
