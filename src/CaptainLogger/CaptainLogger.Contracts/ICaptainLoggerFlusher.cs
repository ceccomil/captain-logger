namespace CaptainLogger.Contracts;

/// <summary>
/// Provides a way to force CaptainLogger to flush any pending file log writes.
/// </summary>
public interface ICaptainLoggerFlusher
{
  /// <summary>
  /// Flushes CaptainLogger synchronously, waiting for any in-flight file write to complete first.
  /// </summary>
  void Flush();

  /// <summary>
  /// Flushes CaptainLogger asynchronously, waiting for any in-flight file write to complete first.
  /// </summary>
  Task FlushAsync(CancellationToken cancellationToken = default);
}
