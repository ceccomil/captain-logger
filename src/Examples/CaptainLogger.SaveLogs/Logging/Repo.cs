using LiteDB;

namespace CaptainLogger.SaveLogs.Logging;

public class Repo : IRepo
{
    private bool _disposed = false;
    
    public LiteDatabase Database { get; }
    public ILiteCollection<LogEntry> LogEntries { get; }

    public Repo(
        IConfiguration configuration)
    {
        Database = new LiteDatabase(configuration["DbFile"]);
        LogEntries = Database.GetCollection<LogEntry>("LogEntries");
    }

    ~Repo() => Dispose(false);

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            Database.Dispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public Task<LogEntry> Add(LogEntry logEntry) => Task
        .FromResult(LogEntries
            .FindById(LogEntries
                .Insert(logEntry)));

    public Task<IEnumerable<LogEntry>> GetAll() => Task
        .FromResult(LogEntries
            .FindAll());
}