namespace CaptainLogger.Logic;

internal class CptLogger : ILogger, IDisposable
{
    private const string INDENT = "                                ";
    private readonly Func<CaptainLoggerOptions> _getCurrentConfig;

    private static FileStream? _fs;
    private static FileInfo? _currentLog = default;
    private static string _timeSuffix = "";
    private static StreamWriter? _sw = default;

    private static readonly object _consoleLock = new();

    public bool Disposed { get; private set; }
    public string Category { get; }

    internal event LogEntryRequestedHandler? OnLogRequested;
    internal event LogEntryRequestedAsyncHandler? OnLogRequestedAsync;

    internal static FileInfo CurrentLog {
        get {
            if (_currentLog is null)
            {
                throw new NullReferenceException(
                    "Current log file must be valid!");
            }

            return _currentLog;
        }
    }

    public CptLogger(
        string name,
        Func<CaptainLoggerOptions> getCurrentConfig)
    {
        Category = name;
        _getCurrentConfig = getCurrentConfig;
    }

    ~CptLogger() => Dispose(false);

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => state as IDisposable ?? null;

    //Default rule and Category Filters still apply
    //TODO consider a "bypass filters in the config options"
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var config = _getCurrentConfig();

        if (config
            .ExcludedEventIds
            .Contains(eventId.Id))
        {
            return;
        }

        var now = DateTime.Now;
        if (config.TimeIsUtc)
        {
            now = DateTime.UtcNow;
        }

        lock (_consoleLock)
        {
            if (OnLogRequested is not null)
            {
                LogHasBeenRequested(
                    now,
                    logLevel,
                    state,
                    eventId,
                    exception);
            }

            if (OnLogRequestedAsync is not null)
            {
                _ = LogHasBeenRequestedAsync(
                    now,
                    logLevel,
                    state,
                    eventId,
                    exception);
            }

            _ = WriteLog(
                now,
                config,
                logLevel,
                state,
                exception,
                formatter);
        }
    }

    private void LogHasBeenRequested<TState>(
        DateTime time,
        LogLevel level,
        TState state,
        EventId eventId,
        Exception? ex)
    {
        if (state is null)
        {
            return;
        }

        OnLogRequested
            ?.Invoke(new(
                state,
                time,
                eventId,
                Category,
                level,
                ex));
    }

    private async Task LogHasBeenRequestedAsync<TState>(
        DateTime time,
        LogLevel level,
        TState state,
        EventId eventId,
        Exception? ex)
    {
        if (state is null || OnLogRequestedAsync is null)
        {
            return;
        }

        await OnLogRequestedAsync
            .Invoke(new(
                state,
                time,
                eventId,
                Category,
                level,
                ex));
    }

    private async Task WriteLog<TState>(
        DateTime time,
        CaptainLoggerOptions config,
        LogLevel level,
        TState state,
        Exception? ex,
        Func<TState, Exception?, string> formatter)
    {
        var row = GetRow(
            time,
            state,
            config.DefaultColor,
            config.LogLevels[level],
            level,
            ex,
            config.DoNotAppendException,
            formatter);

        if (config.LogRecipients.HasFlag(Recipients.Console))
        {
            WriteToConsole(row);
        }

        if (config.LogRecipients.HasFlag(Recipients.File))
        {
            await WriteToLogFile(row, config);
        }

        if (config.LogRecipients.HasFlag(Recipients.Stream))
        {
            await WriteToBuffer(row, config);
        }
    }

    private static void WriteToConsole(RowParts row)
    {
        Console.ForegroundColor = row.TimeStamp.Color;
        Console.Write(row.TimeStamp);

        Console.ForegroundColor = row.Level.Color;
        Console.Write(row.Level);

        Console.ForegroundColor = row.Message.Color;
        Console.Write(row.Message);

        Console.ForegroundColor = row.Category.Color;
        Console.Write(row.Category);

        Console.ForegroundColor = row.Spacer.Color;
        Console.Write(row.Spacer);

        Console.ResetColor();
    }

    private static async Task WriteToLogFile(
        RowParts row,
        CaptainLoggerOptions config)
    {
        CheckLogFileName(row.Time, config);

        if (_sw is null || _fs is null)
        {
            throw new NullReferenceException($"Log filestream must be valid!");
        }

        await _sw.WriteAsync(row.ToString());

        _sw.Flush();
        _fs.Flush();
    }

    private static async Task WriteToBuffer(
        RowParts row,
        CaptainLoggerOptions config)
    {
        if (config.LoggerBuffer is null)
        {
            throw new NullReferenceException($"Log Buffer stream must be a valid opened `System.Stream`!");
        }

        var buffer = Encoding.UTF8.GetBytes(row.ToString());
        await config.LoggerBuffer.WriteAsync(buffer);

        config.LoggerBuffer.Flush();
    }

    private RowParts GetRow<TState>(
        DateTime time,
        TState state,
        ConsoleColor defaultColor,
        ConsoleColor levelColor,
        LogLevel level,
        Exception? ex,
        bool doNotAppendEx,
        Func<TState, Exception?, string> formatter)
    {
        var row = new RowParts()
        {
            Time = time,
            TimeStamp = new(
                $"[{time:yyyy-MM-dd HH:mm:ss.fff}] ",
                ConsoleColor.DarkCyan),
            Level = new(
                $"[{level.ToCaptainLoggerString()}] ",
                levelColor),
            Message = new(
                GetLogMessage(
                    state,
                    ex,
                    doNotAppendEx,
                    formatter),
                defaultColor),
            Category = new(
                $"{INDENT}[{Category}]\r\n",
                ConsoleColor.Magenta),
            Spacer = new(
                "\r\n",
                defaultColor)
        };

        return row;
    }

    private static string GetLogMessage<TState>(
        TState state,
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

    private static void CheckLogFileName(
        DateTime time,
        CaptainLoggerOptions config,
        int? counter = default)
    {
        var tSuffix = config.GetTimeSuffix(time);

        if (_currentLog is not null && _timeSuffix == tSuffix)
        {
            return;
        }

        _timeSuffix = tSuffix;
        var log = config.GetLogFile(time, counter);

        _fs.CloseAndDispose();
        _sw.CloseAndDispose();

        try
        {
            _currentLog = log.InitAndLock(ref _fs, ref _sw);
        }
        catch
        {
            CheckLogFileName(
                time,
                config,
                counter.GetValueOrDefault() + 1);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (Disposed)
        {
            return;
        }

        if (disposing)
        {
            //No private members
        }

        _sw.CloseAndDispose();
        _fs.CloseAndDispose();

        _fs = null;
        _sw = null;
        _currentLog = null;

        Disposed = true;
    }
}
