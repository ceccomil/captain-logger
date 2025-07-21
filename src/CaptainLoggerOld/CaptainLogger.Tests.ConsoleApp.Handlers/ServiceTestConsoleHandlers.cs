
namespace CaptainLogger.Tests.ConsoleApp.Handlers;

public class ServiceTestConsoleHandlers : IServiceTest
{
    private readonly ICaptainLogger _logger;
    public string InstanceId { get; }

    public ServiceTestConsoleHandlers(
        ICaptainLogger<ServiceTest> logger,
        IOptions<ServiceTestOptions> opts)
    {
        _logger = logger;
        InstanceId = opts.Value.InstanceId;
    }

    private async Task SomeLoggingWithEventHandlers()
    {
        void LogEntryRequested(CaptainLoggerEvArgs<object> evArgs)
        {
            //Some long operations
            Thread.Sleep(500);
            Console.WriteLine($"SYNC HANDLER: {evArgs.LogTime:ss.fff} - {evArgs.State}");
        }

        async Task LogEntryRequestedAsync(CaptainLoggerEvArgs<object> evArgs)
        {
            //Some long operations
            await Task.Delay(1000);
            Console.WriteLine($"ASYNC HANDLER: {evArgs.LogTime:ss.fff} - {evArgs.State}");
        }

        //ASYNC HANDLERS
        _logger.LogEntryRequestedAsync += LogEntryRequestedAsync;

        await Task.Delay(100);
        _logger.InformationLog("Simple message no arguments!");

        await Task.Delay(100);
        _logger.ErrorLog("Simple error message", new NotImplementedException("Test Exception"));

        await Task.Delay(3000);
        _logger.LogEntryRequestedAsync -= LogEntryRequestedAsync;

        Console.WriteLine();

        //SYNC HANDLERS
        _logger.LogEntryRequested += LogEntryRequested;

        await Task.Delay(100);
        _logger.InformationLog("Mex for Sync Handler");

        await Task.Delay(100);
        _logger.ErrorLog("Exception for Sync Handler", new NotImplementedException("Test Exception"));

        _logger.LogEntryRequested -= LogEntryRequested;
    }

    public async Task RunAsync() => await SomeLoggingWithEventHandlers();
}
