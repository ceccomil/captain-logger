namespace CaptainLogger.Helpers;

internal static class InternalGlobals
{
  public const string INDENT = "                                ";
  public const int INDENT_LENGTH = 32;

  public const string INDENT_NL = "\n" + INDENT;
  public const string CRLF = "\r\n";
  public const int CRLF_LEN = 2;

  public static char[] CrAndLfArray { get; } = ['\r', '\n'];

  public static Dictionary<LogLevel, ConsoleColor> LogLevels { get; } = new Dictionary<LogLevel, ConsoleColor>()
  {
    [LogLevel.Information] = ConsoleColor.Green,
    [LogLevel.Error] = ConsoleColor.DarkRed,
    [LogLevel.Warning] = ConsoleColor.Yellow,
    [LogLevel.Critical] = ConsoleColor.Red,
    [LogLevel.Debug] = ConsoleColor.Cyan,
    [LogLevel.Trace] = ConsoleColor.DarkCyan
  };
}
