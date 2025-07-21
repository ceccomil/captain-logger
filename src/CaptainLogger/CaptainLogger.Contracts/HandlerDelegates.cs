namespace CaptainLogger.Contracts;

/// <summary>
/// Asynchronous event handler for intercepting log entries with boxed state.
/// </summary>
/// <param name="evArgs">Log entry details with state boxed as object.</param>
public delegate Task LogEntryRequestedAsyncHandler(
  CaptainLoggerEventArgs<object> evArgs);