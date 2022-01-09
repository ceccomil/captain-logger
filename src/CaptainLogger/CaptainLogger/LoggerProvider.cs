using System.Collections.Concurrent;

namespace CaptainLogger;

internal sealed class LoggerProvider : ILoggerProvider
{
    private readonly IDisposable _onChangeToken;
    private LoggerConfigOptions _currentConfig;
    private readonly ConcurrentDictionary<string, CptLogger> _loggers = new();

    private bool _disposed;

    public LoggerProvider(
        IOptionsMonitor<LoggerConfigOptions> config)
    {
        _currentConfig = config.CurrentValue;
        _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
    }

    ~LoggerProvider() => Dispose(false);

    public ILogger CreateLogger(string categoryName) => _loggers
        .GetOrAdd(categoryName, name => new CptLogger(name, GetCurrentConfig));

    private LoggerConfigOptions GetCurrentConfig() => _currentConfig;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _loggers.Clear();
            _onChangeToken.Dispose();
        }

        _disposed = true;
    }
}
