namespace CaptainLogger;

internal sealed class CaptainLogger<TCategory>
  : ICaptainLogger<TCategory>
{
  private static readonly Action<ILogger, string, Exception?> Trace = LoggerMessage
    .Define<string>(LogLevel.Trace, 0, "{message}");

  private static readonly Action<ILogger, string, Exception?> Debug = LoggerMessage
    .Define<string>(LogLevel.Debug, 0, "{message}");

  private static readonly Action<ILogger, string, Exception?> Information = LoggerMessage
    .Define<string>(LogLevel.Information, 0, "{message}");

  private static readonly Action<ILogger, string, Exception?> Warning = LoggerMessage
    .Define<string>(LogLevel.Warning, 0, "{message}");

  private static readonly Action<ILogger, string, Exception?> Error = LoggerMessage
    .Define<string>(LogLevel.Error, 0, "{message}");

  private static readonly Action<ILogger, string, Exception?> Critical = LoggerMessage
    .Define<string>(LogLevel.Critical, 0, "{message}");

  public ILogger RuntimeLogger { get; }

  public FileInfo CurrentLogFile => LogFileSystem.CurrentLog;

  public CaptainLogger(
    ILogger<TCategory> logger,
    ILoggerProvider loggerProvider)
  {
    RuntimeLogger = logger;

    if (loggerProvider is not CaptainLoggerProvider lp)
    {
      throw new ArgumentException(
        $"The provided logger provider must be of type {nameof(CaptainLoggerProvider)}.",
        nameof(loggerProvider));
    }
  }

  public void TraceLog(string message, Exception? exception = null)
  {
    if (!RuntimeLogger.IsEnabled(LogLevel.Trace))
    {
      return;
    }

    Trace(RuntimeLogger, message, exception);
  }

  public void DebugLog(string message, Exception? exception = null)
  {
    if (!RuntimeLogger.IsEnabled(LogLevel.Debug))
    {
      return;
    }

    Debug(RuntimeLogger, message, exception);
  }

  public void InformationLog(string message, Exception? exception = null)
  {
    if (!RuntimeLogger.IsEnabled(LogLevel.Information))
    {
      return;
    }

    Information(RuntimeLogger, message, exception);
  }

  public void WarningLog(string message, Exception? exception = null)
  {
    if (!RuntimeLogger.IsEnabled(LogLevel.Warning))
    {
      return;
    }

    Warning(RuntimeLogger, message, exception);
  }

  public void ErrorLog(string message, Exception? exception = null)
  {
    if (!RuntimeLogger.IsEnabled(LogLevel.Error))
    {
      return;
    }

    Error(RuntimeLogger, message, exception);
  }

  public void CriticalLog(string message, Exception? exception = null)
  {
    if (!RuntimeLogger.IsEnabled(LogLevel.Critical))
    {
      return;
    }

    Critical(RuntimeLogger, message, exception);
  }
}
