
namespace CaptainLogger.Logic;

internal class CptLogger : ILogger
{
    private const string INDENT = "                                ";

    private readonly string _name;
    private readonly Func<LoggerConfigOptions> _getCurrentConfig;

    private static FileStream? _fs;
    private static FileInfo? _currentLog = default;
    private static string _timeSuffix = "";
    private static StreamWriter? _sw = default;

    private static bool _isWriting = false;

    public CptLogger(
        string name,
        Func<LoggerConfigOptions> getCurrentConfig)
    {
        _name = name;
        _getCurrentConfig = getCurrentConfig;
    }

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

            //Do not wait!
            _ = WriteLog(now,
            config,
            logLevel,
            state,
            exception,
            formatter);
    }

    private async Task WriteLog<TState>(
        DateTime time,
        LoggerConfigOptions config,
        LogLevel level,
        TState state,
        Exception? ex,
        Func<TState, Exception?, string> formatter)
    {
        // Make sure the entire log message is written in one iteration
        // otherwise other async calls could inject words into the unit
        while (_isWriting)
            await Task.Delay(50);

        _isWriting = true;

        CheckLogFileName(time, config);

        if (_sw is null || _fs is null)
            throw new NullReferenceException($"Log filestream must be valid!");

        var originalColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        var timeStr = $"[{time:yyyy-MM-dd HH:mm:ss.fff}] ";
        Console.Write(timeStr);
        await _sw.WriteAsync(timeStr);

        Console.ForegroundColor = config.LogLevels[level];
        var levelStr = $"[{level.ToCaptainLoggerString()}] ";
        Console.Write(levelStr);
        await _sw.WriteAsync(levelStr);

        Console.ForegroundColor = originalColor;

        var mex = formatter(state, ex)
            .Replace("\r", "")
            .Replace("\n", $"\n{INDENT}");

        Console.Write(mex);
        await _sw.WriteAsync(mex);
        Console.WriteLine();
        await _sw.WriteLineAsync();

        if (ex is not null)
        {
            var exStr = $"{INDENT}{ex}"
            .Replace("\r", "")
            .Replace("\n", $"\n{INDENT}");

            Console.Write(exStr);
            await _sw.WriteAsync(exStr);
            Console.WriteLine();
            await _sw.WriteLineAsync();
        }

        Console.ForegroundColor = ConsoleColor.Magenta;
        var name = $"{INDENT}[{_name}]\r\n";
        Console.Write(name);
        await _sw.WriteAsync(name);
        Console.ForegroundColor = originalColor;
        Console.WriteLine();
        await _sw.WriteLineAsync();

        _sw.Flush();
        _fs.Flush();

        _isWriting = false;
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
}
