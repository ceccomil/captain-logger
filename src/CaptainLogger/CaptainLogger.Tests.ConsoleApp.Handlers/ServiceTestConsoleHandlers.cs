
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

    private void LogEntryRequested(CaptainLoggerEvArgs<object> evArgs)
    {
        Thread.Sleep(500);
        Console.WriteLine($"SYNC HANDLER: {evArgs.LogTime:ss.fff} - {evArgs.State}");
    }

    private async Task LogEntryRequestedAsync(CaptainLoggerEvArgs<object> evArgs)
    {
        //Some long operation
        await Task.Delay(1000);
        Console.WriteLine($"ASYNC HANDLER: {evArgs.LogTime:ss.fff} - {evArgs.State}");
    }


    public async Task RunAsync()
    {
        _logger.LogEntryRequestedAsync += LogEntryRequestedAsync;

        await Task.Delay(100);

        _logger.InformationLog("Simple message no arguments!");

        await Task.Delay(100);

        _logger.ErrorLog("Simple error message", new NotImplementedException("Test Exception"));

        await Task.Delay(3000);

        Console.WriteLine();

        _logger.LogEntryRequestedAsync -= LogEntryRequestedAsync;
        _logger.LogEntryRequested += LogEntryRequested;

        Run();

        _logger.LogEntryRequested -= LogEntryRequested;
    }

    public void Run()
    {
        _logger.InformationLog("Mex for Sync Handler");

        _logger.ErrorLog("Exception for Sync Handler", new NotImplementedException("Test Exception"));
    }
}
