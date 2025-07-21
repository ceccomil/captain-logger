namespace CaptainLogger.Helpers;

internal static class LoggerExtensions
{
  public static string ToCaptainLoggerString(
    this LogLevel logLevel) => logLevel switch
    {
      LogLevel.Error => "ERR",
      LogLevel.Warning => "WRN",
      LogLevel.Critical => "CRT",
      LogLevel.Debug => "DBG",
      LogLevel.Trace => "TRC",
      _ => "INF",
    };

  public static string ToLogMessage<TState>(
    this TState state,
    Exception? ex,
    bool doNotAppendEx,
    Func<TState, Exception?, string> formatter)
  {
    var mex = formatter(state, ex)
      .Replace("\r", "")
      .Replace("\n", $"\n{INDENT}");

    if (ex is not null && !doNotAppendEx)
    {
      mex += "\r\n" +
        $"{INDENT}{ex}"
        .Replace("\r", "")
        .Replace("\n", $"\n{INDENT}");
    }

    return mex + "\r\n";
  }
}
