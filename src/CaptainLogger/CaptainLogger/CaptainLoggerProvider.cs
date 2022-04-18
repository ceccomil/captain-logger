namespace CaptainLogger;

internal sealed class CaptainLoggerProvider : ILoggerProvider
{
    private readonly IDisposable _onChangeToken;
    public CaptainLoggerOptions CurrentConfig { get; private set; }
    public ConcurrentDictionary<string, CptLogger> Loggers { get; } = new();

    public event EventHandler<NewLoggerEvArgs>? LoggerAdded;

    private bool _disposed;

    public CaptainLoggerProvider(
        IOptionsMonitor<CaptainLoggerOptions> config)
    {
        CurrentConfig = config.CurrentValue;
        _onChangeToken = config.OnChange(updatedConfig => CurrentConfig = updatedConfig);
    }

    ~CaptainLoggerProvider() => Dispose(false);

    public ILogger CreateLogger(string categoryName)
    {
        var logger = Loggers.GetOrAdd(categoryName, name => new CptLogger(name, GetCurrentConfig));

        LoggerAdded?.Invoke(this, new(logger));

        return logger;
    }

    private CaptainLoggerOptions GetCurrentConfig() => CurrentConfig;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            foreach (var logger in Loggers.Values)
            {
                logger.Dispose();
            }

            Loggers.Clear();
            _onChangeToken.Dispose();
        }

        _disposed = true;
    }
}
