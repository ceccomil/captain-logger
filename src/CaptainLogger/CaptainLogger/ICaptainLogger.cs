namespace CaptainLogger;

/// <summary>
/// Provides simplified logging methods and event interception capabilities.
/// <para>Use for quick logging scenarios; for performance-critical cases, use source-generated extensions.</para>
/// </summary>
public interface ICaptainLogger
{
  /// <summary>
  /// Gets the file currently used for logging output.
  /// </summary>
  FileInfo CurrentLogFile { get; }

  /// <summary>
  /// Gets the underlying <see cref="ILogger"/> instance used for log dispatching.
  /// </summary>
  ILogger RuntimeLogger { get; }

  /// <summary>
  /// Writes a <see cref="LogLevel.Trace"/> log entry, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void TraceLog(string message);

  /// <summary>
  /// Writes a <see cref="LogLevel.Trace"/> log entry with an associated exception, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void TraceLog(string message, Exception exception);

  /// <summary>
  /// Writes a <see cref="LogLevel.Debug"/> log entry, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void DebugLog(string message);

  /// <summary>
  /// Writes a <see cref="LogLevel.Debug"/> log entry with an associated exception, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void DebugLog(string message, Exception exception);

  /// <summary>
  /// Writes a <see cref="LogLevel.Information"/> log entry, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void InformationLog(string message);

  /// <summary>
  /// Writes a <see cref="LogLevel.Information"/> log entry with an associated exception, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void InformationLog(string message, Exception exception);

  /// <summary>
  /// Writes a <see cref="LogLevel.Warning"/> log entry, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void WarningLog(string message);

  /// <summary>
  /// Writes a <see cref="LogLevel.Warning"/> log entry with an associated exception, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void WarningLog(string message, Exception exception);

  /// <summary>
  /// Writes a <see cref="LogLevel.Error"/> log entry, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void ErrorLog(string message);

  /// <summary>
  /// Writes a <see cref="LogLevel.Error"/> log entry with an associated exception, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void ErrorLog(string message, Exception exception);

  /// <summary>
  /// Writes a <see cref="LogLevel.Critical"/> log entry, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void CriticalLog(string message);

  /// <summary>
  /// Writes a <see cref="LogLevel.Critical"/> log entry with an associated exception, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void CriticalLog(string message, Exception exception);

  /// <summary>
  /// Raised when a log entry is requested.
  /// <para>Exceptions thrown in handlers are not propagated to avoid recursive logging failures.</para>
  /// </summary>
  /// <remarks>
  /// Logging must be fail-safe. If a handler throws, the logger will swallow the exception to prevent a recursive
  /// failure loop (e.g., attempting to log an error that occurred during logging).
  /// 
  /// Handlers should log responsibly and avoid introducing instability or blocking operations.
  /// This mechanism can be used for log propagation (e.g., centralized processing), but should be designed defensively.
  /// </remarks>
  event LogEntryRequestedAsyncHandler? LogEntryRequestedAsync;
}

/// <summary>
/// Variant of <see cref="ICaptainLogger"/> where the logger category name is derived from the <typeparamref name="TCategory"/> type.
/// </summary>
/// <typeparam name="TCategory">The type used to determine the logging category name.</typeparam>
/// <remarks>
/// This variant is commonly used for context-aware logging or when resolving loggers via dependency injection,
/// allowing log entries to be automatically categorized by the associated type.
/// </remarks>
public interface ICaptainLogger<out TCategory> : ICaptainLogger { }
