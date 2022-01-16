
namespace CaptainLogger;

internal class CaptainLoggerBase<TCategory> : ICaptainLogger<TCategory>
{
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

    public ILogger RuntimeLogger { get; }

    public CaptainLoggerBase(ILogger<TCategory> logger)
    {
        RuntimeLogger = logger;
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
}
