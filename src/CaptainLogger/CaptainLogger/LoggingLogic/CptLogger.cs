namespace CaptainLogger.LoggingLogic;

internal sealed class CptLogger(
  string name,
  Func<CaptainLoggerOptions> getCurrentConfig) : ILogger
{
  private readonly Func<CaptainLoggerOptions> _getCurrentConfig = getCurrentConfig;

  private static readonly object _consoleLock = new();

  public string Category { get; } = name;

  public event LogEntryRequestedAsyncHandler? OnLogRequestedAsync;

  public IDisposable? BeginScope<TState>(TState state)
    where TState : notnull => state as IDisposable ?? null;

  // Default filters rules still apply!
  public bool IsEnabled(LogLevel logLevel) => true;

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
          exception);
      }

      _ = WriteLog(
        now,
        config,
        logLevel,
        state,
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

    await OnLogRequestedAsync.Invoke(evArgs);
  }

  private async Task WriteLog<TState>(
    DateTime time,
    CaptainLoggerOptions config,
    LogLevel level,
    TState state,
    Exception? ex,
    Func<TState, Exception?, string> formatter)
  {
    var line = GetLogLine(
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
      WriteToConsole(line);
    }

    if (config.LogRecipients.HasFlag(Recipients.File))
    {
      await line.WriteToLogFile(config);
    }

    if (config.LogRecipients.HasFlag(Recipients.Stream))
    {
      await WriteToBuffer(line, config);
    }
  }

  private static void WriteToConsole(LogLine line)
  {
    Console.ForegroundColor = line.TimeStamp.Color;
    Console.Write(line.TimeStamp);

    Console.ForegroundColor = line.Level.Color;
    Console.Write(line.Level);

    Console.ForegroundColor = line.Message.Color;
    Console.Write(line.Message);

    Console.ForegroundColor = line.Category.Color;
    Console.Write(line.Category);

    Console.ForegroundColor = line.Spacer.Color;
    Console.Write(line.Spacer);

    Console.ResetColor();
  }

  private static async Task WriteToBuffer(
    LogLine line,
    CaptainLoggerOptions config)
  {
    if (config.LoggerBuffer is null)
    {
      throw new InvalidOperationException(
        "Log Buffer stream must be a valid opened `System.Stream`!");
    }

    var buffer = Encoding.UTF8.GetBytes(line.ToString());
    await config.LoggerBuffer.WriteAsync(buffer);

    config.LoggerBuffer.Flush();
  }

  private LogLine GetLogLine<TState>(
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

    var line = new LogLine(
      time,
      new($"[{time:yyyy-MM-dd HH:mm:ss.fff}] ", ConsoleColor.DarkCyan),
      new($"[{level.ToCaptainLoggerString()}] ", levelColor),
      new(message, defaultColor),
      new($"{INDENT}[{Category}]\r\n", ConsoleColor.Magenta),
      new("\r\n", defaultColor));

    return line;
  }
}
