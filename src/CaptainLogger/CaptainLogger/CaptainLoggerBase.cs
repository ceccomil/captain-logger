namespace CaptainLogger;

internal class CaptainLoggerBase<TCategory> : ICaptainLogger<TCategory>, IDisposable
{
    private bool _disposed;

    private static readonly Action<ILogger, string, Exception?> Trace = LoggerMessage
        .Define<string>(LogLevel.Trace, 0, "{Message}");

    private static readonly Action<ILogger, string, Exception?> Debug = LoggerMessage
        .Define<string>(LogLevel.Debug, 0, "{Message}");

    private static readonly Action<ILogger, string, Exception?> Information = LoggerMessage
        .Define<string>(LogLevel.Information, 0, "{Message}");

    private static readonly Action<ILogger, string, Exception?> Warning = LoggerMessage
        .Define<string>(LogLevel.Warning, 0, "{Message}");

    private static readonly Action<ILogger, string, Exception?> Error = LoggerMessage
        .Define<string>(LogLevel.Error, 0, "{Message}");

    private static readonly Action<ILogger, string, Exception?> Critical = LoggerMessage
        .Define<string>(LogLevel.Critical, 0, "{Message}");

    public event LogEntryRequestedHandler? LogEntryRequested;
    public event LogEntryRequestedAsyncHandler? LogEntryRequestedAsync;

    private readonly ConcurrentDictionary<string, CptLogger> _loggers;
    private readonly CaptainLoggerOptions _options;

    public ILogger RuntimeLogger { get; }

    public CaptainLoggerBase(
        ILogger logger,
        ILoggerProvider loggerProvider)
    {
        RuntimeLogger = logger;
        _loggers = ((CaptainLoggerProvider)loggerProvider).Loggers;

        _options = ((CaptainLoggerProvider)loggerProvider).CurrentConfig;

        foreach (var cpt in _loggers.Values)
        {
            if (_options.TriggerEvents)
                cpt.OnLogRequested += CptLoggerOnLogRequested;

            if (_options.TriggerAsyncEvents)
                cpt.OnLogRequestedAsync += CptLoggerOnLogRequestedAsync;
        }
    }

    public CaptainLoggerBase(
        ILogger<TCategory> logger,
        ILoggerProvider ILoggerProvider) :
        this((ILogger)logger, ILoggerProvider) { }

    ~CaptainLoggerBase() => Dispose(false);

    private void CptLoggerOnLogRequested(CaptainLoggerEvArgs<object> evArgs)
    {
        if (LogEntryRequested is null)
            return;

        LogEntryRequested.Invoke(evArgs);
    }

    private async Task CptLoggerOnLogRequestedAsync(CaptainLoggerEvArgs<object> evArgs)
    {
        if (LogEntryRequestedAsync is null)
            return;

        await LogEntryRequestedAsync.Invoke(evArgs);
    }

    public void TraceLog(string message) => Trace(RuntimeLogger, message, null);

    public void TraceLog(string message, Exception exception) => Trace(RuntimeLogger, message, exception);

    public void DebugLog(string message) => Debug(RuntimeLogger, message, null);

    public void DebugLog(string message, Exception exception) => Debug(RuntimeLogger, message, exception);

    public void InformationLog(string message) => Information(RuntimeLogger, message, null);

    public void InformationLog(string message, Exception exception) => Information(RuntimeLogger, message, exception);

    public void WarningLog(string message) => Warning(RuntimeLogger, message, null);

    public void WarningLog(string message, Exception exception) => Warning(RuntimeLogger, message, exception);

    public void ErrorLog(string message) => Error(RuntimeLogger, message, null);

    public void ErrorLog(string message, Exception exception) => Error(RuntimeLogger, message, exception);

    public void CriticalLog(string message) => Critical(RuntimeLogger, message, null);

    public void CriticalLog(string message, Exception exception) => Critical(RuntimeLogger, message, exception);
    
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
            foreach (var cpt in _loggers.Values)
            {
                if (_options.TriggerEvents)
                    cpt.OnLogRequested -= CptLoggerOnLogRequested;

                if (_options.TriggerAsyncEvents)
                    cpt.OnLogRequestedAsync -= CptLoggerOnLogRequestedAsync;
            }
        }

        _disposed = true;
    }
}
