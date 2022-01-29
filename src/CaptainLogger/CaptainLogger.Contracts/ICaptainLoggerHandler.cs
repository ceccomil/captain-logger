using CaptainLogger.Contracts.EventArguments;

namespace CaptainLogger.Contracts;

public interface ICaptainLoggerHandler
{
    Task LogEntryRequested<TState>(CaptainLoggerEvArgs<TState> evArgs);
}
