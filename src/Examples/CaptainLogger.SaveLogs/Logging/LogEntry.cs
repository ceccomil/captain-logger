namespace CaptainLogger.SaveLogs.Logging;

public record LogEntry(
    DateTime TimeStamp,
    LogLevel LogLevel,
    string Message,
    string Category,
    string? ExceptionMessage,
    string? ExceptionStackTrace)
{
}