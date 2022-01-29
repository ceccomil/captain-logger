namespace CaptainLogger.Logic;

internal class CptLogger : ILogger, IDisposable
{
    private const string INDENT = "                                ";

    private readonly string _name;
    private readonly Func<CaptainLoggerOptions> _getCurrentConfig;

    private static FileStream? _fs;
    private static FileInfo? _currentLog = default;
    private static string _timeSuffix = "";
    private static StreamWriter? _sw = default;

    private static readonly object _consoleLock = new();

    private readonly List<ICaptainLoggerHandler> _handlers = new();

    public bool Disposed { get; private set; }

    public CptLogger(
        string name,
        Func<CaptainLoggerOptions> getCurrentConfig,
        IServiceProvider sp)
    {
        _name = name;
        _getCurrentConfig = getCurrentConfig;

        var handlers = sp
            .TryGetServices<ICaptainLoggerHandler>();

        _handlers
            .AddRange(handlers);
    }

    ~CptLogger() => Dispose(false);

    public IDisposable BeginScope<TState>(TState state) => state as IDisposable ?? null!;

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
            return;

        var config = _getCurrentConfig();

        if (config.EventId != eventId.Id)
            return;

        var now = DateTime.Now;
        if (config.TimeIsUtc)
            now = DateTime.UtcNow;

        lock (_consoleLock)
        {
            _ = LogHasBeenRequested(
                now,
                logLevel,
                state,
                eventId,
                exception);

            _ = WriteLog(
                now,
                config,
                logLevel,
                state,
                exception,
                formatter);
        }
    }

    private async Task LogHasBeenRequested<TState>(
        DateTime time,
        LogLevel level,
        TState state,
        EventId eventId,
        Exception? ex)
    {
        foreach (var handler in _handlers)
            await handler
                .LogEntryRequested<TState>(new(
                    state,
                    time,
                    eventId,
                    _name,
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
            WriteToConsole(row);

        if (config.LogRecipients.HasFlag(Recipients.File))
            await WriteToLogFile(row, config);

        if (config.LogRecipients.HasFlag(Recipients.Stream))
            await WriteToBuffer(row, config);
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
    }

    private static async Task WriteToLogFile(
        RowParts row,
        CaptainLoggerOptions config)
    {
        CheckLogFileName(row.Time, config);

        if (_sw is null || _fs is null)
            throw new NullReferenceException($"Log filestream must be valid!");

        await _sw.WriteAsync(row.ToString());

        _sw.Flush();
        _fs.Flush();
    }

    private static async Task WriteToBuffer(
        RowParts row,
        CaptainLoggerOptions config)
    {
        if (config.LoggerBuffer is null)
            throw new NullReferenceException($"Log Buffer stream must be a valid opened `System.Stream`!");

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
                $"{INDENT}[{_name}]{Environment.NewLine}",
                ConsoleColor.Magenta),
            Spacer = new(
                Environment.NewLine,
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
            mex += Environment.NewLine +
                $"{INDENT}{ex}"
                .Replace("\r", "")
                .Replace("\n", $"\n{INDENT}");

        return mex + Environment.NewLine;
    }

    private static void CheckLogFileName(
        DateTime time,
        CaptainLoggerOptions config,
        int? counter = default)
    {
        var tSuffix = config.GetTimeSuffix(time);

        if (_currentLog is not null && _timeSuffix == tSuffix)
            return;

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
            return;

        if (disposing)
        {
            //No private members
        }

        _fs.CloseAndDispose();
        _sw.CloseAndDispose();

        _fs = null;
        _sw = null;
        _currentLog = null;

        Disposed = true;
    }
}
