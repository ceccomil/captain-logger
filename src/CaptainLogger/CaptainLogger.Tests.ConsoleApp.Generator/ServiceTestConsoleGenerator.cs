namespace CaptainLogger.Tests.ConsoleApp.Generator;

public class ServiceTestConsoleGenerator : IServiceTest
{
    private readonly ICaptainLogger _logger;

    public string InstanceId { get; }

    public ServiceTestConsoleGenerator(
        ICaptainLogger<ServiceTest> logger,
        IOptions<ServiceTestOptions> opts)
    {
        _logger = logger;
        InstanceId = opts.Value.InstanceId;
    }

    public async Task RunAsync()
    {
        _logger.InfoLog("Simple message no arguments!");

        await Task.Delay(1000);

        _logger.ErrorLog("Simple error message", new NotImplementedException("Test Exception"));

        await Task.Delay(1000);

        _logger.TraceLog(
            "Simple message 1 argument {Guid}!",
            Guid.NewGuid());

        await Task.Delay(1000);

        _logger.TraceLog("Simple error message 1 argument {Guid}",
            Guid.NewGuid(),
            new NotImplementedException("Test Exception"));

        await Task.Delay(1000);
    }
}

