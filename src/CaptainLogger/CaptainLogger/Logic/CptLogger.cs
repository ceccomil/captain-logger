namespace CaptainLogger.Logic;

internal class CptLogger : ILogger, IDisposable
{
    private const string INDENT = "                                ";

    private readonly string _name;
    private readonly Func<LoggerConfigOptions> _getCurrentConfig;

    private static FileStream? _fs;
    private static FileInfo? _currentLog = default;
    private static string _timeSuffix = "";
    private static StreamWriter? _sw = default;

    private static readonly object _consoleLock = new();

    public bool Disposed { get; private set; }

    public CptLogger(
        string name,
        Func<LoggerConfigOptions> getCurrentConfig)
    {
        _name = name;
        _getCurrentConfig = getCurrentConfig;
    }

    ~CptLogger() => Dispose(false);

    public IDisposable BeginScope<TState>(TState state) => state as IDisposable ?? null!;

    //Default rule and Category Filters still apply
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
            //Do not wait!
            _ = WriteLog(now,
                config,
                logLevel,
                state,
                exception,
                formatter);
        }
    }

    private async Task WriteLog<TState>(
        DateTime time,
        LoggerConfigOptions config,
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
        LoggerConfigOptions config)
    {
        CheckLogFileName(row.Time, config);

        if (_sw is null || _fs is null)
            throw new NullReferenceException($"Log filestream must be valid!");

        await _sw.WriteAsync(row.ToString());

        _sw.Flush();
        _fs.Flush();
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
        LoggerConfigOptions config,
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
