namespace CaptainLogger.Contracts.Helpers;

internal static class InternalGlobals
{
  public const string INDENT = "                                ";

  public static Dictionary<LogLevel, ConsoleColor> LogLevels { get; } = new Dictionary<LogLevel, ConsoleColor>()
  {
    [LogLevel.Information] = ConsoleColor.Green,
    [LogLevel.Error] = ConsoleColor.DarkRed,
    [LogLevel.Warning] = ConsoleColor.Yellow,
    [LogLevel.Critical] = ConsoleColor.Red,
    [LogLevel.Debug] = ConsoleColor.Cyan,
    [LogLevel.Trace] = ConsoleColor.DarkCyan
  };

  public static string DefaultLogName { get; } =
    $"./Logs/{AppDomain.CurrentDomain.FriendlyName.Replace(".", "-")}.log";

  public static FileInfo GetLogFile(
    this CaptainLoggerOptions options,
    DateTime time,
    int? counter = default) => options
      .FilePath
      .GetLogFile(options.FileRotation, time, counter);

  public static FileInfo GetLogFile(
    this string filePath,
    LogRotation fileRotation,
    DateTime time,
    int? counter = default)
  {
    var fullPath = Path.GetFullPath(filePath);
    var dirPath = Path.GetDirectoryName(fullPath);

    if (string.IsNullOrWhiteSpace(dirPath))
    {
      dirPath = Path.GetFullPath("./Logs");
    }

    var file = Path.GetFileNameWithoutExtension(fullPath);
    var ext = Path.GetExtension(fullPath);

    if (!Directory.Exists(dirPath))
    {
      Directory.CreateDirectory(dirPath);
    }

    if (string.IsNullOrWhiteSpace(ext) || ext == ".")
    {
      ext = ".log";
    }

    var fileNoExt = Path.Combine(dirPath, $"{file}{fileRotation.GetTimeSuffix(time)}");

    if (counter.GetValueOrDefault() > 0)
    {
      fileNoExt += $"_{counter:000}";
    }

    return new FileInfo($"{fileNoExt}{ext}");
  }

  public static string GetTimeSuffix(
    this LogRotation fileRotation,
    DateTime time) => fileRotation switch
    {
      LogRotation.Year => $"-{time:yyyy}",
      LogRotation.Month => $"-{time:yyyyMM}",
      LogRotation.Day => $"-{time:yyyyMMdd}",
      LogRotation.Hour => $"-{time:yyyyMMddHH}",
      LogRotation.Minute => $"-{time:yyyyMMddHHmm}",
      _ => ""
    };
}
