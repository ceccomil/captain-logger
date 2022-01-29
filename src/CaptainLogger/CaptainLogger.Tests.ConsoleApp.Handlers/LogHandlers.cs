namespace CaptainLogger.Tests.ConsoleApp.Handlers;

internal class LogHandler1 : ICaptainLoggerHandler
{
    public async Task LogEntryRequested<TState>(CaptainLoggerEvArgs<TState> evArgs) =>
        //Something will happen e.g. DB logging
        await Task.Delay(10);
}

internal class LogHandler2 : ICaptainLoggerHandler
{
    public async Task LogEntryRequested<TState>(CaptainLoggerEvArgs<TState> evArgs) =>
        //Something really slow will happen e.g. Remote logging
        await Task.Delay(2_000);
}
