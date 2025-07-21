namespace CaptainLogger.Contracts;

/// <summary>
/// Generic asynchronous event handler for intercepting log entries.
/// </summary>
/// <typeparam name="TState">The type of the log state object.</typeparam>
/// <param name="evArgs">Log entry details.</param>
/// <param name="cancellationToken">Token to observe cancellation requests.</param>
public delegate Task LogEntryRequestedAsyncHandler<TState>(
  CaptainLoggerEventArgs<TState> evArgs,
  CancellationToken cancellationToken);


/// <summary>
/// Asynchronous event handler for intercepting log entries with boxed state.
/// </summary>
/// <param name="evArgs">Log entry details with state boxed as object.</param>
/// <param name="cancellationToken">Token to observe cancellation requests.</param>
public delegate Task LogEntryRequestedAsyncHandler(
  CaptainLoggerEventArgs<object> evArgs,
  CancellationToken cancellationToken);