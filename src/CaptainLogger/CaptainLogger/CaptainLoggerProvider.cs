namespace CaptainLogger;

internal sealed class CaptainLoggerProvider : ILoggerProvider
{
  private readonly IDisposable? _onChangeToken;

  public CaptainLoggerOptions CurrentConfig { get; private set; }
  public ConcurrentDictionary<string, CptLogger> Loggers { get; } = [];

  public event LoggerAddedHandler? LoggerAdded;

  private bool _disposed;

  public CaptainLoggerProvider(
    IOptionsMonitor<CaptainLoggerOptions> config)
  {
    CurrentConfig = config.CurrentValue;

    _onChangeToken = config.OnChange(x => CurrentConfig = x);
  }

  public ILogger CreateLogger(string categoryName)
  {
    var logger = Loggers.GetOrAdd(categoryName, name => new CptLogger(name, GetCurrentConfig));

    LoggerAdded?.Invoke(logger);

    return logger;
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  private void Dispose(bool disposing)
  {
    if (_disposed || !disposing)
    {
      return;
    }

    foreach (var logger in Loggers.Values)
    {
      logger.Dispose();
    }

    Loggers.Clear();
    _onChangeToken?.Dispose();

    _disposed = true;
  }

  private CaptainLoggerOptions GetCurrentConfig() => CurrentConfig;
}