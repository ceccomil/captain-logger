using CaptainLogger.Contracts.EventArguments;

namespace CaptainLogger.Contracts;

/// <summary>
/// Simple handler for log entries
/// </summary>
public interface ICaptainLoggerHandler
{
    /// <summary>
    /// Triggered by any log entry before they are written.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="evArgs"></param>
    Task LogEntryRequested<TState>(CaptainLoggerEvArgs<TState> evArgs);
}
