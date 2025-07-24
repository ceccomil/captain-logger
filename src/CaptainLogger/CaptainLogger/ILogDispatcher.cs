namespace CaptainLogger;

/// <summary>
/// Provides a centralized mechanism to observe log entries as they are written.
/// Used for forwarding logs to custom sinks (e.g., database, telemetry, etc.).
/// </summary>
public interface ILogDispatcher
{
  /// <summary>
  /// Occurs when a log entry is written by any logger using this provider.
  /// Subscribers can use this event to process, forward, or persist log data.
  /// </summary>
  event LogEntryRequestedAsyncHandler? OnLogEntry;
}
