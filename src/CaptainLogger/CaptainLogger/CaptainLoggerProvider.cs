namespace CaptainLogger;

internal sealed class CaptainLoggerProvider : ILoggerProvider
{
  private readonly IDisposable? _onChangeToken;

  public CaptainLoggerOptions CurrentConfig { get; private set; }
  public ConcurrentDictionary<string, CptLoggerBase> Loggers { get; } = [];

  private bool _disposed;

  public CaptainLoggerProvider(
    IOptionsMonitor<CaptainLoggerOptions> config)
  {
    CurrentConfig = config.CurrentValue;
    _onChangeToken = config.OnChange(x => CurrentConfig = x);
  }

  public ILogger CreateLogger(string categoryName) =>
    GetOrAddLogger(categoryName);

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

    Loggers.Clear();
    _onChangeToken?.Dispose();

    _disposed = true;
  }

  private CaptainLoggerOptions GetCurrentConfig() => CurrentConfig;

  private CptLoggerBase GetOrAddLogger(string categoryName)
  {
    if (CurrentConfig.HighPerfStructuredLogging)
    {
      return Loggers.GetOrAdd(categoryName, name => new JsonCptLogger(name, GetCurrentConfig));
    }

    return Loggers.GetOrAdd(categoryName, name => new CptLogger(name, GetCurrentConfig));
  }
}