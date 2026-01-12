using ModularisTest.Enums;
using ModularisTest.Interfaces;
using System;
using System.Configuration;
using System.IO;

namespace ModularisTest.Destinations
{
    public class FileLogDestination : ILogDestination
    {
        private readonly string _logFileDirectory;

        public FileLogDestination(string logFileDirectory = null)
        {
            _logFileDirectory = logFileDirectory ?? 
                ConfigurationManager.AppSettings["LogFileDirectory"] ?? 
                Environment.CurrentDirectory;
        }

        public void LogMessage(string message, LogMessageType messageType)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var trimmedMessage = message.Trim();
            if (string.IsNullOrEmpty(trimmedMessage))
            {
                return;
            }

            var logFileName = "LogFile" + DateTime.Now.ToShortDateString().Replace("/", ".") + ".txt";
            var logFullFilePath = Path.Combine(_logFileDirectory, logFileName);

            string existingContent = string.Empty;
            if (File.Exists(logFullFilePath))
            {
                existingContent = File.ReadAllText(logFullFilePath);
            }

            var typeLabel = GetTypeLabel(messageType);
            var logEntry = DateTime.Now.ToShortDateString() + " " + typeLabel + " " + trimmedMessage + Environment.NewLine;
            var newContent = existingContent + logEntry;

            if (!Directory.Exists(_logFileDirectory))
            {
                Directory.CreateDirectory(_logFileDirectory);
            }

            File.WriteAllText(logFullFilePath, newContent);
        }

        private string GetTypeLabel(LogMessageType messageType)
        {
            switch (messageType)
            {
                case LogMessageType.Error:
                    return "Error  ";
                case LogMessageType.Warning:
                    return "Warning";
                case LogMessageType.Message:
                    return "Message";
                default:
                    return "Message";
            }
        }
    }
}
