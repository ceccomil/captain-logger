﻿namespace CaptainLogger.Options;

/// <summary>
/// Options used to configure behavior for <c>CaptainLogger</c>.
/// </summary>
public class CaptainLoggerOptions : IDisposable
{
    private bool _disposed;

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
    /// <para><see cref="LogArguments.One"/> -> <c>InformationLog(string message, T0 arg0)</c></para>
    /// <para><see cref="LogArguments.Two"/> -> <c>InforomationLog(string message, T0 arg0, T1 arg1)</c>...</para>
    /// <para>!!Requires <c>CaptainLogger.Extensions.Generator</c> package.</para>
    /// </summary>
    public LogArguments ArgumentsCount { get; set; } = LogArguments.Zero;

    /// <summary>
    /// If <see cref="ArgumentsCount"/> is greater than zero, a custom list of templates can be provided.
    /// <para>i.e. <see cref="LogArguments.One"/> template: <c>`This is the log template used for one argument: {Arg0}`</c></para>
    /// </summary>
    public IDictionary<LogArguments, string> Templates { get; } = new Dictionary<LogArguments, string>();

    /// <summary>
    /// A stream to catch the logs if <see cref="LogRecipients"/> has flag <see cref="Recipients.Stream"/>
    /// </summary>
    public Stream? LoggerBuffer { get; set; }

    /// <summary>
    /// If set to false, <see cref="ICaptainLogger.LogEntryRequestedAsync"/> won't be triggered!
    /// <para>Default value: False</para>
    /// </summary>
    public bool TriggerAsyncEvents { get; set; }

    /// <summary>
    /// If set to false, <see cref="ICaptainLogger.LogEntryRequested"/> won't be triggered!
    /// <para>Default value: False</para>
    /// </summary>
    public bool TriggerEvents { get; set; }

    /// <summary>
    /// Providing a list prevents any log raised from being written if it matches any of the specified ids
    /// </summary>
    public IEnumerable<int> ExcludedEventIds { get; set; } = Enumerable.Empty<int>();

    /// <summary>
    /// Default constructor
    /// </summary>
    public CaptainLoggerOptions() { }

    /// <summary>
    /// Finalizer
    /// </summary>
    ~CaptainLoggerOptions() => Dispose(false);

    internal FileInfo GetLogFile(DateTime time, int? counter = default)
    {
        var pathDirectory = Path.GetDirectoryName(FilePath);

        if (string.IsNullOrEmpty(pathDirectory))
        {
            pathDirectory = ".//Logs";
        }

        var dir = Path.GetFullPath(pathDirectory);
        var file = Path.GetFileNameWithoutExtension(FilePath);
        var ext = Path.GetExtension(FilePath);

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if (string.IsNullOrEmpty(ext) || ext == ".")
        {
            ext = ".log";
        }

        var fileNoExt = Path.Combine(dir, $"{file}{GetTimeSuffix(time)}");

        if (counter.GetValueOrDefault() > 0)
        {
            fileNoExt += $"_{counter:000}";
        }

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

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing) { }

        if (LoggerBuffer is not null)
        {
            LoggerBuffer.Close();
            LoggerBuffer.Dispose();
            LoggerBuffer = null;
        }

        _disposed = true;
    }

    /// <summary>
    /// Dispose managed and unmanaged members
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
