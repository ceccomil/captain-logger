namespace CaptainLogger;

/// <summary>
/// Provides a correlation ID scope mechanism for log enrichment across asynchronous operations.
/// </summary>
public static class CaptainLoggerCorrelationScope
{
  private static readonly AsyncLocal<string?> _correlationId = new();

  /// <summary>
  /// Attempts to retrieve the current correlation ID in scope.
  /// </summary>
  /// <param name="correlationId">The correlation ID if available; otherwise, <c>null</c>.</param>
  /// <returns><c>true</c> if a correlation ID is set; otherwise, <c>false</c>.</returns>
  public static bool TryGetCorrelationId(out string? correlationId)
  {
    correlationId = _correlationId.Value;
    return !string.IsNullOrWhiteSpace(correlationId);
  }

  /// <summary>
  /// Begins a new correlation scope with the specified GUID-based correlation ID.
  /// </summary>
  /// <param name="correlationId">The GUID to use as a correlation ID.</param>
  /// <returns>An <see cref="IDisposable"/> that restores the previous scope upon disposal.</returns>
  public static IDisposable BeginScope(Guid correlationId)
  {
    var stringId = correlationId.ToString();
    return BeginScope(stringId);
  }

  /// <summary>
  /// Begins a new correlation scope with the specified correlation ID.
  /// The scope is restored to the previous value when disposed.
  /// </summary>
  /// <param name="correlationId">The correlation ID to use in the new scope.</param>
  /// <returns>An <see cref="IDisposable"/> that restores the previous scope upon disposal.</returns>
  /// <exception cref="ArgumentException">Thrown if the correlation ID is null or whitespace.</exception>
  public static IDisposable BeginScope(string correlationId)
  {
    if (string.IsNullOrWhiteSpace(correlationId))
    {
      throw new ArgumentException(
        "Correlation ID cannot be null or whitespace.",
        nameof(correlationId));
    }

    var previous = _correlationId.Value;
    _correlationId.Value = correlationId;

    var scope = new DisposableScope(previous);

    return scope;
  }

  private sealed record DisposableScope(string? RestoreValue) : IDisposable
  {
    public void Dispose()
    {
      _correlationId.Value = RestoreValue;
      GC.SuppressFinalize(this);
    }
  }
}
