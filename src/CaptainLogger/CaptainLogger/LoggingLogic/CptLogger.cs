namespace CaptainLogger.LoggingLogic;

internal sealed class CptLogger(
  string name,
  Func<CaptainLoggerOptions> getCurrentConfig) : ILogger, IDisposable
{
  private readonly Func<CaptainLoggerOptions> _getCurrentConfig = getCurrentConfig;

  private static FileStream? _fs;
  private static FileInfo? _currentLog = default;
  private static string _timeSuffix = "";
  private static StreamWriter? _sw = default;

  private static readonly object _consoleLock = new();

  // TODO: Use a more robust cancellation token source management
  private readonly CancellationTokenSource _cts = new();

  public bool Disposed { get; private set; }
  public string Category { get; } = name;

  public event LogEntryRequestedAsyncHandler? OnLogRequestedAsync;

  public static FileInfo CurrentLog
  {
    get
    {
      if (_currentLog is null)
      {
        throw new InvalidOperationException(
          "Current log file must be valid!");
      }

      return _currentLog;
    }
  }

  public IDisposable? BeginScope<TState>(TState state)
    where TState : notnull => state as IDisposable ?? null;

  // Default filters rules still apply!
  public bool IsEnabled(LogLevel logLevel) => true;

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  public void Log<TState>(
    LogLevel logLevel,
    EventId eventId,
    TState state,
    Exception? exception,
    Func<TState, Exception?, string> formatter)
  {
    var config = _getCurrentConfig();

    if (config
      .ExcludedEventIds
      .Contains(eventId.Id))
    {
      return;
    }

    var now = GetCurrentTime(config);

    lock (_consoleLock)
    {
      if (OnLogRequestedAsync is not null)
      {
        _ = LogHasBeenRequestedAsync(
          now,
          logLevel,
          state,
          eventId,
          exception,
          _cts.Token);
      }

      _ = WriteLog(
        now,
        config,
        logLevel,
        state,
        exception,
        formatter,
        _cts.Token);
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
    Exception? ex,
    CancellationToken cancellationToken)
  {
    if (state is null || OnLogRequestedAsync is null)
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

    await OnLogRequestedAsync.Invoke(evArgs, cancellationToken);
  }

  private async Task WriteLog<TState>(
    DateTime time,
    CaptainLoggerOptions config,
    LogLevel level,
    TState state,
    Exception? ex,
    Func<TState, Exception?, string> formatter,
    CancellationToken cancellationToken)
  {
    var row = GetRow(
      time,
      state,
      config.DefaultColor,
      InternalGlobals.LogLevels[level],
      level,
      ex,
      config.DoNotAppendException,
      formatter);

    if (config.LogRecipients.HasFlag(Recipients.Console))
    {
      WriteToConsole(row);
    }

    if (config.LogRecipients.HasFlag(Recipients.File))
    {
      await WriteToLogFile(row, config, cancellationToken);
    }

    if (config.LogRecipients.HasFlag(Recipients.Stream))
    {
      await WriteToBuffer(row, config, cancellationToken);
    }
  }

  private static void WriteToConsole(RowParts row)
  {
    Console.ForegroundColor = row.TimeStamp.Color;
    Console.Write(row.TimeStamp);

    Console.ForegroundColor = row.Level.Color;
    Console.Write(row.Level);

    Console.ForegroundColor = row.Message.Color;
    Console.Write(row.Message);

    Console.ForegroundColor = row.Category.Color;
    Console.Write(row.Category);

    Console.ForegroundColor = row.Spacer.Color;
    Console.Write(row.Spacer);

    Console.ResetColor();
  }

  private static async Task WriteToLogFile(
    RowParts row,
    CaptainLoggerOptions config,
    CancellationToken cancellationToken)
  {
    CheckLogFileName(row.Time, config);

    if (_sw is null || _fs is null)
    {
      throw new InvalidOperationException("Log filestream must be valid!");
    }

    await _sw.WriteAsync(row.Content, cancellationToken);

    _sw.Flush();
    _fs.Flush();
  }

  private static async Task WriteToBuffer(
    RowParts row,
    CaptainLoggerOptions config,
    CancellationToken cancellationToken)
  {
    if (config.LoggerBuffer is null)
    {
      throw new InvalidOperationException(
        "Log Buffer stream must be a valid opened `System.Stream`!");
    }

    var buffer = Encoding.UTF8.GetBytes(row.ToString());
    await config.LoggerBuffer.WriteAsync(buffer, cancellationToken);

    config.LoggerBuffer.Flush();
  }

  private RowParts GetRow<TState>(
    DateTime time,
    TState state,
    ConsoleColor defaultColor,
    ConsoleColor levelColor,
    LogLevel level,
    Exception? ex,
    bool doNotAppendEx,
    Func<TState, Exception?, string> formatter)
  {
    var message = state.ToLogMessage(
      ex,
      doNotAppendEx,
      formatter);

    var row = new RowParts(
      time,
      new($"[{time:yyyy-MM-dd HH:mm:ss.fff}] ", ConsoleColor.DarkCyan),
      new($"[{level.ToCaptainLoggerString()}] ", levelColor),
      new(message, defaultColor),
      new($"{INDENT}[{Category}]\r\n", ConsoleColor.Magenta),
      new("\r\n", defaultColor));

    return row;
  }

  private static void CheckLogFileName(
    DateTime time,
    CaptainLoggerOptions config,
    int? counter = default)
  {
    var tSuffix = config.FileRotation.GetTimeSuffix(time);

    if (_currentLog is not null && _timeSuffix == tSuffix)
    {
      return;
    }

    _timeSuffix = tSuffix;
    var log = config.GetLogFile(time, counter);

    _fs.CloseAndDispose();
    _sw.CloseAndDispose();

    try
    {
      _currentLog = log.InitAndLock(ref _fs, ref _sw);
    }
    catch
    {
      CheckLogFileName(
        time,
        config,
        counter.GetValueOrDefault() + 1);
    }
  }

  private void Dispose(bool disposing)
  {
    if (Disposed || !disposing)
    {
      return;
    }

    _cts.Dispose();
    _sw.CloseAndDispose();
    _fs.CloseAndDispose();

    _fs = null;
    _sw = null;
    _currentLog = null;

    Disposed = true;
  }
}
