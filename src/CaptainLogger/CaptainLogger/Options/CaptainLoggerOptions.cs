namespace CaptainLogger.Options;

/// <summary>
/// Options used to configure behavior for <c>CaptainLogger</c>.
/// </summary>
public class CaptainLoggerOptions
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
    /// Default Color for messages written on the Console.
    /// </summary>
    public ConsoleColor DefaultColor { get; set; } = Console.ForegroundColor;

    /// <summary>
    /// If set to <c>True</c> log lines timestamps will be UTC
    /// </summary>
    public bool TimeIsUtc { get; set; }

    /// <summary>
    /// If set to <c>True</c> a valid exception passed, won't be appended
    /// <para><c>Formatter</c> function won't be affected
    /// <see cref="ILogger.Log{TState}(LogLevel, Microsoft.Extensions.Logging.EventId, TState, Exception?, Func{TState, Exception?, string})"/></para> 
    /// </summary>
    public bool DoNotAppendException { get; set; }

    /// <summary>
    /// Determines where log lines should be written.
    /// <para>The default is <see cref="Recipients.File"/> and <see cref="Recipients.Console"/> </para>
    /// </summary>
    public Recipients LogRecipients { get; set; } = Recipients.Console | Recipients.File;

    /// <summary>
    /// Log file path
    /// <para>The default is <c>./Logs/AssmeblyName.log</c> </para>
    /// </summary>
    public string FilePath { get; set; } = $".//Logs/{AppDomain.CurrentDomain.FriendlyName.Replace(".", "-")}.log";

    /// <summary>
    /// Appends the specified interval onto the log file name before the <c>extension</c>
    /// <para>The default is <see cref="LogRotation.Hour"/> </para>
    /// </summary>
    public LogRotation FileRotation { get; set; } = LogRotation.Hour;

    /// <summary>
    /// Add log extensions methods equal to the <see cref="LogArguments"/> count specified.
    /// <para><see cref="LogArguments.One"/> -> <c>InfoLog(string message, T0 arg0)</c></para>
    /// <para><see cref="LogArguments.Two"/> -> <c>InfoLog(string message, T0 arg0, T1 arg1)</c>...</para>
    /// <para>!!Requires <c>CaptainLogger.Contracts.Generator</c> package.</para>
    /// </summary>
    public LogArguments ArgumentsCount { get; set; } = LogArguments.Zero;

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
