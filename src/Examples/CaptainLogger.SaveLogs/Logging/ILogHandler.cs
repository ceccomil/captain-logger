namespace CaptainLogger.SaveLogs.Logging;

public interface ILogHandler : IDisposable
{
    void SubscribeToLoggerEvents();
}