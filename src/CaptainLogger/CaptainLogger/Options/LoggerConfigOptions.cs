namespace CaptainLogger.Options;

/// <summary>
/// 
/// </summary>
public class LoggerConfigOptions
{
    internal int EventId { get; } = 0;

    internal IDictionary<LogLevel, ConsoleColor> LogLevels { get; } = new Dictionary<LogLevel, ConsoleColor>()
    {
        [LogLevel.Information] = ConsoleColor.Green,
        [LogLevel.Error] = ConsoleColor.DarkRed,
        [LogLevel.Warning] = ConsoleColor.Yellow,
        [LogLevel.Critical] = ConsoleColor.Red,
        [LogLevel.Debug] = ConsoleColor.Cyan,
        [LogLevel.Trace] = ConsoleColor.DarkCyan
    };

    /// <summary>
    /// 
    /// </summary>
    public ConsoleColor DefaultColor { get; set; } = Console.ForegroundColor;

    /// <summary>
    /// 
    /// </summary>
    public bool TimeIsUtc { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool DoNotAppendException { get; set; } 

    /// <summary>
    /// 
    /// </summary>
    public Recipients LogRecipients { get; set; } = Recipients.Console | Recipients.File;

    /// <summary>
    /// 
    /// </summary>
    public string FilePath { get; set; } = $".//Logs/{AppDomain.CurrentDomain.FriendlyName.Replace(".", "-")}.log";

    /// <summary>
    /// 
    /// </summary>
    public LogRotation FileRotation { get; set; } = LogRotation.Hour;

    internal FileInfo GetLogFile(DateTime time, int? counter = default)
    {
        var pathDirectory = Path.GetDirectoryName(FilePath);

        if (string.IsNullOrEmpty(pathDirectory))
            pathDirectory = ".//Logs";

        var dir = Path.GetFullPath(pathDirectory);
        var file = Path.GetFileNameWithoutExtension(FilePath);
        var ext = Path.GetExtension(FilePath);

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        if (string.IsNullOrEmpty(ext) || ext == ".")
            ext = ".log";

        var fileNoExt = Path.Combine(dir, $"{file}{GetTimeSuffix(time)}");

        if (counter.GetValueOrDefault() > 0)
            fileNoExt += $"_{counter:000}";

        return new FileInfo($"{fileNoExt}{ext}");
    }


    internal string GetTimeSuffix(DateTime time) => FileRotation switch
    {
        LogRotation.Year => $"-{time:yyyy}",
        LogRotation.Month => $"-{time:yyyyMM}",
        LogRotation.Day => $"-{time:yyyyMMdd}",
        LogRotation.Hour => $"-{time:yyyyMMddHH}",
        LogRotation.Minute => $"-{time:yyyyMMddHHmm}",
        _ => ""
    };
}
