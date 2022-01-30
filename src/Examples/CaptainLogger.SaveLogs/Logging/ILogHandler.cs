namespace CaptainLogger.SaveLogs.Logging;

public interface ILogHandler<out TCategory> : IDisposable
{
    void SubscribeToLoggerEvents();
}