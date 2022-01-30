using System.Diagnostics;

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

        _logger.LogEntryRequestedAsync += LogEntryRequestedAsync;
    }

    private async Task LogEntryRequestedAsync(CaptainLoggerEvArgs<object> evArgs) =>
        //Some long operation
        await Task.Delay(1000);

    public async Task RunAsync()
    {
        _logger.InformationLog("Simple message no arguments!");

        await Task.Delay(100);

        _logger.ErrorLog("Simple error message", new NotImplementedException("Test Exception"));

        await Task.Delay(4000);
    }
}
