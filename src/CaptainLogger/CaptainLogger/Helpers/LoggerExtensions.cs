namespace CaptainLogger.Helpers;

internal static class LoggerExtensions
{
  private const string ERR = "ERR";
  private const string WRN = "WRN";
  private const string CRT = "CRT";
  private const string DBG = "DBG";
  private const string TRC = "TRC";
  private const string INF = "INF";

  public static string ToCaptainLoggerString(
    this LogLevel logLevel) => logLevel switch
    {
      LogLevel.Error => ERR,
      LogLevel.Warning => WRN,
      LogLevel.Critical => CRT,
      LogLevel.Debug => DBG,
      LogLevel.Trace => TRC,
      _ => INF,
    };

  public static string ToLogMessage<TState>(
    this TState state,
    Exception? ex,
    bool doNotAppendEx,
    Func<TState, Exception?, string> formatter)
  {
    var baseMsg = formatter(state, null);
    var haveEx = ex is not null && !doNotAppendEx;

    //Fastlane for simple messages
    if (!haveEx && baseMsg.IndexOfAny(CrAndLfArray) < 0)
    {
      return baseMsg + CRLF;
    }

    // 2 is equal to CRLF
    var exLen = haveEx
      ? ex!.ToString().Length + INDENT_LENGTH + CRLF_LEN
      : 0;

    int capacityHint =
      baseMsg.Length +
      exLen +
      CRLF_LEN;

    var sb = StringBuilderCache.GetNewOrCached(capacityHint);

    AppendWithIndent(sb, baseMsg);

    if (haveEx)
    {
      sb
        .Append(CRLF)
        .Append(INDENT);

      AppendWithIndent(sb, ex!.ToString());
    }

    sb.Append(CRLF);

    return StringBuilderCache.GetStringAndCacheIt(sb);
  }

  private static void AppendWithIndent(StringBuilder sb, string text)
  {
    var span = text.AsSpan();
    int start = 0;

    while (true)
    {
      var nl = span[start..].IndexOf(CrAndLfArray[1]);

      if (nl < 0)
      {
        sb.Append(span[start..]);
        break;
      }

      sb.Append(span.Slice(start, nl));
      sb.Append(INDENT_NL);
      start += nl + 1;
    }
  }
}
