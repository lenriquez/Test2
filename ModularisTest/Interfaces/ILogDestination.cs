using ModularisTest.Enums;

namespace ModularisTest.Interfaces
{
    public interface ILogDestination
    {
        void LogMessage(string message, LogMessageType messageType);
    }
}
