namespace CaptainLogger.LoggingLogic;

internal sealed class CptLogger(
  string name,
  Func<CaptainLoggerOptions> getCurrentConfig)
  : CptLoggerBase(name, getCurrentConfig)
{
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
    Console.ForegroundColor = line.TimeStamp.Color;
    Console.Write(line.TimeStamp);

    Console.ForegroundColor = line.Level.Color;
    Console.Write(line.Level);

    Console.ForegroundColor = line.Message.Color;
    Console.Write(line.Message);

    Console.ForegroundColor = line.CorrelationId.Color;
    Console.Write(line.CorrelationId);

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

    var correlationId = "";

    if (CaptainLoggerCorrelationScope.TryGetCorrelationId(
      out var correlationIdValue))
    {
      correlationId = $"{INDENT}[{correlationIdValue}]\r\n";
    }

    var line = new LogLine(
      time,
      new($"[{time:yyyy-MM-dd HH:mm:ss.fff}] ", ConsoleColor.DarkCyan),
      new($"[{level.ToCaptainLoggerString()}] ", levelColor),
      new(message, defaultColor),
      new($"{INDENT}[{Category}]\r\n", ConsoleColor.Magenta),
      new(correlationId, ConsoleColor.DarkMagenta),
      new("\r\n", defaultColor));

    return line;
  }
}
