using ModularisTest.Enums;
using ModularisTest.Interfaces;
using System;

namespace ModularisTest.Destinations
{
    public class ConsoleLogDestination : ILogDestination
    {
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

            var originalColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = GetConsoleColor(messageType);
                Console.WriteLine(DateTime.Now.ToShortDateString() + trimmedMessage);
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }

        private ConsoleColor GetConsoleColor(LogMessageType messageType)
        {
            switch (messageType)
            {
                case LogMessageType.Error:
                    return ConsoleColor.Red;
                case LogMessageType.Warning:
                    return ConsoleColor.Yellow;
                case LogMessageType.Message:
                    return ConsoleColor.White;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}
