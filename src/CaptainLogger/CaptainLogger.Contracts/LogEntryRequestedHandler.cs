using CaptainLogger.Contracts.EventArguments;

namespace CaptainLogger.Contracts;

/// <summary>
/// Event handler used to intercept log entries.
/// </summary>
/// <param name="evArgs"></param>
/// <returns></returns>
public delegate void LogEntryRequestedHandler(CaptainLoggerEvArgs<object> evArgs);

/// <summary>
/// Event handler used to intercept log entries (async not awaited!).
/// </summary>
/// <param name="evArgs"></param>
/// <returns></returns>
public delegate Task LogEntryRequestedAsyncHandler(CaptainLoggerEvArgs<object> evArgs);
