namespace CaptainLogger.LoggingLogic;

internal abstract class CptLoggerBase(
  string category,
  string provider,
  Func<CaptainLoggerOptions> getCurrentConfig,
  Func<LoggerFilterOptions> getCurrentFilters) : ILogger
{
  private static readonly object _consoleLock = new();

  private Func<LoggerFilterOptions> GetCurrentFilters { get; } = getCurrentFilters;
  protected Func<CaptainLoggerOptions> GetCurrentConfig { get; } = getCurrentConfig;

  public string Category { get; } = category;
  public string ProviderName { get; } = provider;

  public event LogEntryRequestedAsyncHandler? OnLogRequestedAsync;

  public IDisposable? BeginScope<TState>(TState state)
    where TState : notnull => state as IDisposable ?? null;

  public bool IsEnabled(LogLevel logLevel)
  {
    var filters = GetCurrentFilters();

    foreach (var rule in filters.Rules)
    {
      if (!IsProviderMatch(rule.ProviderName))
      {
        continue;
      }

      if (!IsCategoryMatch(rule.CategoryName))
      {
        continue;
      }

      if (rule.LogLevel.HasValue && logLevel < rule.LogLevel.Value)
      {
        return false;
      }

      if (rule.Filter is not null && !rule.Filter(ProviderName, Category, logLevel))
      {
        return false;
      }

      return true;
    }

    return logLevel >= filters.MinLevel;
  }

  protected abstract Task WriteLog<TState>(
    DateTime time,
    CaptainLoggerOptions config,
    LogLevel level,
    TState state,
    EventId eventId,
    Exception? ex,
    Func<TState, Exception?, string> formatter);

  public void Log<TState>(
    LogLevel logLevel,
    EventId eventId,
    TState state,
    Exception? exception,
    Func<TState, Exception?, string> formatter)
  {
    var config = GetCurrentConfig();

    if (config
      .ExcludedEventIds
      .Contains(eventId.Id))
    {
      return;
    }

    var now = GetCurrentTime(config);

    _ = LogHasBeenRequestedAsync(
      now,
      logLevel,
      state,
      eventId,
      exception);

    lock (_consoleLock)
    {
      _ = WriteLog(
        now,
        config,
        logLevel,
        state,
        eventId,
        exception,
        formatter);
    }
  }

  private static DateTime GetCurrentTime(CaptainLoggerOptions config)
  {
    return config.TimeIsUtc
      ? DateTime.UtcNow
      : DateTime.Now;
  }

  private async Task LogHasBeenRequestedAsync<TState>(
    DateTime time,
    LogLevel level,
    TState state,
    EventId eventId,
    Exception? ex)
  {
    if (OnLogRequestedAsync is null || state is null)
    {
      return;
    }

    CaptainLoggerCorrelationScope
      .TryGetCorrelationId(out var correlationId);

    var evArgs = new CaptainLoggerEventArgs<object>(
      state,
      time,
      eventId,
      Category,
      level,
      correlationId,
      ex);

    await OnLogRequestedAsync.Invoke(evArgs);
  }

  private bool IsProviderMatch(string? ruleProvider)
  {
    return string.IsNullOrEmpty(ruleProvider) || ProviderName.Equals(
      ruleProvider,
      StringComparison.OrdinalIgnoreCase);
  }

  private bool IsCategoryMatch(string? ruleCategory)
  {
    return string.IsNullOrEmpty(ruleCategory) || Category.StartsWith(
      ruleCategory,
      StringComparison.OrdinalIgnoreCase);
  }
}
