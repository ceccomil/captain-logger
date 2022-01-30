using Microsoft.Extensions.Logging;

namespace CaptainLogger.Contracts.EventArguments;

/// <summary>
/// Arguments of the log entry.
/// </summary>
public class CaptainLoggerEvArgs<TState> : EventArgs
{
    /// <summary>
    /// The object to be written by the injected <see cref="ILogger"/>.
    /// </summary>
    public TState State { get; }

    /// <summary>
    /// Creation time of the log entry.
    /// <para>Depending on the configuration, could be local or UTC</para>
    /// </summary>
    public DateTime LogTime { get; }

    /// <summary>
    /// The Id and optionally the name of the event associated to the log entry.
    /// </summary>
    public EventId LogEvent { get; }

    /// <summary>
    /// The category associated to the log entry.
    /// </summary>
    public string LogCategory { get; }

    /// <summary>
    /// Severity level of the log entry.
    /// </summary>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// The exception associated to the log entry.
    /// </summary>
    public Exception? Exception { get; }

    internal CaptainLoggerEvArgs(
        TState state,
        DateTime logTime,
        EventId eventId,
        string category,
        LogLevel level,
        Exception? exception = default
        ) : base()
    {
        State = state;
        LogTime = logTime;
        LogEvent = eventId;
        LogCategory = category;
        LogLevel = level;
        Exception = exception;
    }
}