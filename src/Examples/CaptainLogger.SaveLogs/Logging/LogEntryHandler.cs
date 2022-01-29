using CaptainLogger.Contracts;
using CaptainLogger.Contracts.EventArguments;

namespace CaptainLogger.SaveLogs.Logging;

public class LogEntryHandler : ICaptainLoggerHandler
{
    private readonly IRepo _repo;

    public LogEntryHandler(
        IRepo repo)
    {
        _repo = repo;
    }

    public async Task LogEntryRequested<TState>(CaptainLoggerEvArgs<TState> evArgs)
    {
        var entry = new LogEntry(
            evArgs.LogTime,
            evArgs.LogLevel,
            $"{evArgs.State}",
            evArgs.LogCategory,
            evArgs.Exception?.Message,
            evArgs.Exception?.ToString());

        await _repo.Add(entry);
    }
}
