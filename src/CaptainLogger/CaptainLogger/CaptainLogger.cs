namespace CaptainLogger;

internal sealed class CaptainLogger<TCategory>
  : ICaptainLogger<TCategory>, IDisposable
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

  public event LogEntryRequestedAsyncHandler? LogEntryRequestedAsync;

  private readonly ConcurrentDictionary<string, CptLogger> _loggers;
  private readonly CaptainLoggerOptions _options;
  private readonly CaptainLoggerProvider _loggerProvider;

  private readonly List<string> _subscribedAsync = [];

  public ILogger RuntimeLogger { get; }

  public FileInfo CurrentLogFile => CptLogger.CurrentLog;

  public CaptainLogger(
    ILogger logger,
    ILoggerProvider loggerProvider)
  {
    RuntimeLogger = logger;
    _loggerProvider = (CaptainLoggerProvider)loggerProvider;
    _loggers = _loggerProvider.Loggers;
    _options = _loggerProvider.CurrentConfig;

    foreach (var cpt in _loggers.Values)
    {
      NewLoggerAdded(cpt);
    }

    _loggerProvider.LoggerAdded += NewLoggerAdded;
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

    foreach (var cat in _subscribedAsync)
    {
      var cpt = _loggers.Values.Single(x => x.Category == cat);
      cpt.OnLogRequestedAsync -= CptLoggerOnLogRequestedAsync;
      cpt.Dispose();
    }

    _subscribedAsync.Clear();

    _loggerProvider.LoggerAdded -= NewLoggerAdded;

    _disposed = true;
  }

  private void NewLoggerAdded(CptLogger logger)
  {
    if (_options.TriggerAsyncEvents)
    {
      SetupAsyncSub(logger);
    }
  }

  private void SetupAsyncSub(CptLogger cpt)
  {
    if (_subscribedAsync.Contains(cpt.Category))
    {
      return;
    }

    _subscribedAsync.Add(cpt.Category);
    cpt.OnLogRequestedAsync += CptLoggerOnLogRequestedAsync;
  }

  private async Task CptLoggerOnLogRequestedAsync(
    CaptainLoggerEventArgs<object> evArgs,
    CancellationToken cancellationToken)
  {
    if (LogEntryRequestedAsync is null)
    {
      return;
    }

    await LogEntryRequestedAsync.Invoke(evArgs, cancellationToken);
  }
}
