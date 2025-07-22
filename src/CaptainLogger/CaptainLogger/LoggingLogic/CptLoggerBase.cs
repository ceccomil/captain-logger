namespace CaptainLogger.LoggingLogic;

internal abstract class CptLoggerBase(
  string name,
  Func<CaptainLoggerOptions> getCurrentConfig) : ILogger
{
  private static readonly object _consoleLock = new();

  protected Func<CaptainLoggerOptions> GetCurrentConfig { get; } = getCurrentConfig;

  public string Category { get; } = name;

  public event LogEntryRequestedAsyncHandler? OnLogRequestedAsync;

  public IDisposable? BeginScope<TState>(TState state)
    where TState : notnull => state as IDisposable ?? null;

  // Category filters still apply.
  public bool IsEnabled(LogLevel logLevel) => true;

  protected abstract Task WriteLog<TState>(
    DateTime time,
    CaptainLoggerOptions config,
    LogLevel level,
    TState state,
    EventId eventId,
    Exception? ex,
    Func<TState, Exception?, string> formatter);

  public void Log<TState>(
    LogLevel logLevel,
    EventId eventId,
    TState state,
    Exception? exception,
    Func<TState, Exception?, string> formatter)
  {
    var config = GetCurrentConfig();

    if (config
      .ExcludedEventIds
      .Contains(eventId.Id))
    {
      return;
    }

    var now = GetCurrentTime(config);

    _ = LogHasBeenRequestedAsync(
      now,
      logLevel,
      state,
      eventId,
      exception);

    lock (_consoleLock)
    {
      _ = WriteLog(
        now,
        config,
        logLevel,
        state,
        eventId,
        exception,
        formatter);
    }
  }

  private static DateTime GetCurrentTime(CaptainLoggerOptions config)
  {
    return config.TimeIsUtc
      ? DateTime.UtcNow
      : DateTime.Now;
  }

  private async Task LogHasBeenRequestedAsync<TState>(
    DateTime time,
    LogLevel level,
    TState state,
    EventId eventId,
    Exception? ex)
  {
    if (OnLogRequestedAsync is null || state is null)
    {
      return;
    }

    var evArgs = new CaptainLoggerEventArgs<object>(
      state,
      time,
      eventId,
      Category,
      level,
      ex);

    await OnLogRequestedAsync.Invoke(evArgs);
  }
}
