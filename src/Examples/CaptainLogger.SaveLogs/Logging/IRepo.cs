using LiteDB;

namespace CaptainLogger.SaveLogs.Logging;

public interface IRepo : IDisposable
{
    LiteDatabase Database { get; }
    ILiteCollection<LogEntry> LogEntries { get; }

    Task<LogEntry> Add(LogEntry logEntry);
    Task<IEnumerable<LogEntry>> GetAll();
}
