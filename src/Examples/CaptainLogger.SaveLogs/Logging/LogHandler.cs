using CaptainLogger.Contracts.EventArguments;

namespace CaptainLogger.SaveLogs.Logging;

public class LogHandler<TCategory> : ILogHandler<TCategory>
{
    private readonly IRepo _loggerRepo;
    private readonly ICaptainLogger _logger;

    private bool _disposed;
    private bool _subscribed;

    public LogHandler(
        IRepo loggerRepo,
        ICaptainLogger<TCategory> logger)
    {
        _loggerRepo = loggerRepo;
        _logger = logger;
    }

    ~LogHandler() => Dispose(false);

    private async Task LogEntryRequestedAsync(CaptainLoggerEvArgs<object> evArgs)
    {
        var entry = new LogEntry(
            evArgs.LogTime,
            evArgs.LogLevel,
            $"{evArgs.State}",
            evArgs.LogCategory,
            evArgs.Exception?.Message,
            evArgs.Exception?.ToString());

        await _loggerRepo.Add(entry);
    }

    public void SubscribeToLoggerEvents()
    {
        if (_subscribed)
            return;

        _logger.LogEntryRequestedAsync += LogEntryRequestedAsync;
        _subscribed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            if (_subscribed)
                _logger.LogEntryRequestedAsync -= LogEntryRequestedAsync;
            
            _loggerRepo.Dispose();
        }

        _disposed = true;
    }
}
