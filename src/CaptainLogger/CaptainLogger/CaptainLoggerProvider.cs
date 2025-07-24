namespace CaptainLogger;

internal sealed class CaptainLoggerProvider : ILoggerProvider, ILogDispatcher
{
  private readonly IDisposable? _onOptionsChangeToken;
  private readonly IDisposable? _onFiltersChangeToken;

  private volatile CaptainLoggerOptions _currentConfig;
  private volatile LoggerFilterOptions _currentFilters;
  private bool _disposed;

  public event LogEntryRequestedAsyncHandler? OnLogEntry;

  public ConcurrentDictionary<string, CptLoggerBase> Loggers { get; } = [];

  public CaptainLoggerProvider(
    IOptionsMonitor<CaptainLoggerOptions> config,
    IOptionsMonitor<LoggerFilterOptions> filters)
  {
    _currentConfig = config.CurrentValue;
    _onOptionsChangeToken = config.OnChange(x =>
      Interlocked.Exchange(ref _currentConfig, x));

    _currentFilters = filters.CurrentValue;
    _onFiltersChangeToken = filters.OnChange(x =>
      Interlocked.Exchange(ref _currentFilters, x));
  }

  public CaptainLoggerOptions GetCurrentConfig() => _currentConfig;
  public LoggerFilterOptions GetCurrentFilters() => _currentFilters;

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
    _onOptionsChangeToken?.Dispose();
    _onFiltersChangeToken?.Dispose();

    _disposed = true;
  }

  private Task LogEntryCreated(CaptainLoggerEventArgs<object> args)
  {
    if (OnLogEntry is null)
    {
      return Task.CompletedTask;
    }

    return OnLogEntry.Invoke(args);
  }

  private CptLoggerBase GetOrAddLogger(string categoryName)
  {
    if (_currentConfig.HighPerfStructuredLogging)
    {
      return Loggers.GetOrAdd(categoryName, category => new JsonCptLogger(
        category,
        _currentConfig.ProviderName,
        GetCurrentConfig,
        GetCurrentFilters,
        LogEntryCreated));
    }

    return Loggers.GetOrAdd(categoryName, category => new CptLogger(
      category,
      _currentConfig.ProviderName,
      GetCurrentConfig,
      GetCurrentFilters,
      LogEntryCreated));
  }
}