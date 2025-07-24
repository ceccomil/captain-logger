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
  /// Writes a <see cref="LogLevel.Trace"/> log entry with an associated exception, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void TraceLog(string message, Exception? exception = null);

  /// <summary>
  /// Writes a <see cref="LogLevel.Debug"/> log entry with an associated exception, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void DebugLog(string message, Exception? exception = null);

  /// <summary>
  /// Writes a <see cref="LogLevel.Information"/> log entry with an associated exception, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void InformationLog(string message, Exception? exception = null);

  /// <summary>
  /// Writes a <see cref="LogLevel.Warning"/> log entry with an associated exception, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void WarningLog(string message, Exception? exception = null);

  /// <summary>
  /// Writes a <see cref="LogLevel.Error"/> log entry with an associated exception, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void ErrorLog(string message, Exception? exception = null);

  /// <summary>
  /// Writes a <see cref="LogLevel.Critical"/> log entry with an associated exception, if enabled.
  /// <para>Use source generator extensions for structured or high-performance logging.</para>
  /// </summary>
  void CriticalLog(string message, Exception? exception = null);
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
