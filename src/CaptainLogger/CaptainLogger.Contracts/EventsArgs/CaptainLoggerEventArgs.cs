namespace CaptainLogger.Contracts.EventsArgs;

/// <summary>
/// Arguments of the log entry.
/// </summary>
public sealed class CaptainLoggerEventArgs<TState> : EventArgs
{
  /// <summary>
  /// The state or message object to be logged, passed to the configured <see cref="ILogger"/>.
  /// </summary>
  public TState State { get; }

  /// <summary>
  /// The timestamp when the log entry was created.
  /// <para>May be local or UTC, depending on configuration.</para>
  /// </summary>
  public DateTime LogTime { get; }

  /// <summary>
  /// Identifies the event being logged, optionally including a name.
  /// </summary>
  public EventId LogEvent { get; }

  /// <summary>
  /// The category name that classifies the log entry (typically the source class or context).
  /// </summary>
  public string LogCategory { get; }

  /// <summary>
  /// Indicates the importance or severity of the log entry (e.g., Information, Warning, Error).
  /// </summary>
  public LogLevel LogLevel { get; }

  /// <summary>
  /// An optional exception related to the log entry, if one was thrown.
  /// </summary>
  public Exception? Exception { get; }

  internal CaptainLoggerEventArgs(
    TState state,
    DateTime logTime,
    EventId eventId,
    string category,
    LogLevel level,
    Exception? exception = default)
    : base()
  {
    State = state;
    LogTime = logTime;
    LogEvent = eventId;
    LogCategory = category;
    LogLevel = level;
    Exception = exception;
  }
}
