namespace CaptainLogger.LoggingLogic;

internal sealed class CptLogger(
  string category,
  string provider,
  Func<CaptainLoggerOptions> getCurrentConfig,
  Func<LoggerFilterOptions> getCurrentFilters,
  Func<CaptainLoggerEventArgs<object>, Task> onLogEntry)
  : CptLoggerBase(
    category,
    provider,
    getCurrentConfig,
    getCurrentFilters,
    onLogEntry)
{
  private readonly LogSegment _categorySegement = new(
    $"{INDENT}[{category}]" + CRLF,
    ConsoleColor.Magenta);

  private readonly LogSegment _spacer = new(
    CRLF,
    getCurrentConfig().DefaultColor);

  protected override async Task WriteLog<TState>(
    DateTime time,
    CaptainLoggerOptions config,
    LogLevel level,
    TState state,
    EventId eventId,
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
    static void AppendAnsi(StringBuilder sb, LogSegment seg)
    {
      sb
        .Append(AnsiPrefix[(int)seg.Color])
        .Append(seg.Value)
        .Append(RESET);
    }

    if (!ConsoleColourPolicy.UseAnsi)
    {
      Console.Write(line.ToString());
      return;
    }

    var sb = StringBuilderCache.GetNewOrCached(line.LineLength + 32);

    AppendAnsi(sb, line.TimeStamp);
    AppendAnsi(sb, line.Level);
    AppendAnsi(sb, line.Message);
    AppendAnsi(sb, line.CorrelationId);
    AppendAnsi(sb, line.Category);
    AppendAnsi(sb, line.Spacer);

    Console.Write(StringBuilderCache.GetStringAndCacheIt(sb));
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

    var data = line.AsSpan();
    var byteCount = Encoding.UTF8.GetByteCount(data);
    var rented = ArrayPool<byte>.Shared.Rent(byteCount);
    var written = Encoding.UTF8.GetBytes(data, rented);
    await config.LoggerBuffer.WriteAsync(rented.AsMemory(0, written));
    ArrayPool<byte>.Shared.Return(rented);
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

    var correlationId = "";

    if (CaptainLoggerCorrelationScope.TryGetCorrelationId(
      out var correlationIdValue))
    {
      correlationId = $"{INDENT}[{correlationIdValue}]" + CRLF;
    }

    var line = new LogLine(
      time,
      new($"[{time:yyyy-MM-dd HH:mm:ss.fff}] ", ConsoleColor.DarkCyan),
      new($"[{level.ToCaptainLoggerString()}] ", levelColor),
      new(message, defaultColor),
      _categorySegement,
      new(correlationId, ConsoleColor.DarkMagenta),
      _spacer);

    return line;
  }
}
