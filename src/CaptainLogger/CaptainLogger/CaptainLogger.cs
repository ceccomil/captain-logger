namespace CaptainLogger;

internal sealed class CaptainLogger<TCategory>
  : ICaptainLogger<TCategory>, IDisposable
{
  private bool _disposed;

  private static readonly Action<ILogger, string, Exception?> Trace = LoggerMessage
    .Define<string>(LogLevel.Trace, 0, "{value}");

  private static readonly Action<ILogger, string, Exception?> Debug = LoggerMessage
    .Define<string>(LogLevel.Debug, 0, "{value}");

  private static readonly Action<ILogger, string, Exception?> Information = LoggerMessage
    .Define<string>(LogLevel.Information, 0, "{value}");

  private static readonly Action<ILogger, string, Exception?> Warning = LoggerMessage
    .Define<string>(LogLevel.Warning, 0, "{value}");

  private static readonly Action<ILogger, string, Exception?> Error = LoggerMessage
    .Define<string>(LogLevel.Error, 0, "{value}");

  private static readonly Action<ILogger, string, Exception?> Critical = LoggerMessage
    .Define<string>(LogLevel.Critical, 0, "{value}");

  public event LogEntryRequestedAsyncHandler? LogEntryRequestedAsync;

  private readonly CptLoggerBase _cptLogger;

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

    var category = typeof(TCategory).FullName
      ?? throw new InvalidOperationException(
        "Unable to determine the logger category name from" +
        " the generic type parameter. Ensure TCategory is" +
        " a valid type with a non-null FullName.");

    _cptLogger = lp.Loggers[category];

    if (lp.CurrentConfig.TriggerAsyncEvents)
    {
      _cptLogger.OnLogRequestedAsync += CptLoggerOnLogRequestedAsync;
    }
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
    if (_disposed || !disposing)
    {
      return;
    }

    _cptLogger.OnLogRequestedAsync -= CptLoggerOnLogRequestedAsync;

    _disposed = true;
  }

  private Task CptLoggerOnLogRequestedAsync(
    CaptainLoggerEventArgs<object> evArgs)
  {
    if (LogEntryRequestedAsync is null)
    {
      return Task.CompletedTask;
    }

    return LogEntryRequestedAsync.Invoke(evArgs);
  }

  public void TempInfo<T1, T2>(T1 arg1, T2 arg2) =>
    TempDefs<T1, T2>.TempInfo(RuntimeLogger, arg1, arg2);
}

internal static class TempDefs<T1, T2>
{
  private static readonly Action<ILogger, T1, T2, Exception?> TempInfoDefine = LoggerMessage
    .Define<T1, T2>(LogLevel.Information, 0, "Simple mex for two values ({firstValue}, {secondValue})");

  internal static void TempInfo(
    ILogger logger,
    T1 arg1,
    T2 arg2) => TempInfoDefine(logger, arg1, arg2, null);
}
