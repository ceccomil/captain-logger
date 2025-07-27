namespace CaptainLogger.Helpers;

internal static class InternalGlobals
{
  public const string INDENT = "                                ";
  public const int INDENT_LENGTH = 32;

  public const string INDENT_NL = "\n" + INDENT;
  public const string CRLF = "\r\n";
  public const int CRLF_LEN = 2;

  public const string ORIGINAL_FORMAT = "{OriginalFormat}";
  public const string MESSAGE = "message";

  public const string RESET = "\u001b[0m";

  public static double FlushIntervalTicks { get; } = Stopwatch.Frequency * 5.0D;

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

  public static string[] AnsiPrefix { get; } =
  [
    "\u001b[30m", // 0  Black
    "\u001b[34m", // 1  DarkBlue
    "\u001b[32m", // 2  DarkGreen
    "\u001b[36m", // 3  DarkCyan
    "\u001b[31m", // 4  DarkRed
    "\u001b[35m", // 5  DarkMagenta
    "\u001b[33m", // 6  DarkYellow
    "\u001b[37m", // 7  Gray
    "\u001b[90m", // 8  DarkGray   (bright black)
    "\u001b[94m", // 9  Blue       (bright blue)
    "\u001b[92m", // 10 Green      (bright green)
    "\u001b[96m", // 11 Cyan       (bright cyan)
    "\u001b[91m", // 12 Red        (bright red)
    "\u001b[95m", // 13 Magenta    (bright magenta)
    "\u001b[93m", // 14 Yellow     (bright yellow)
    "\u001b[97m"  // 15 White      (bright white)
  ];
}
