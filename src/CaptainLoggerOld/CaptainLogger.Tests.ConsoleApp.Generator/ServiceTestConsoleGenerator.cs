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
        _logger.InformationLog("Simple message no arguments!");

        _logger.InformationLog(arg0: "Simple but from template!");

        await Task.Delay(100);

        _logger.ErrorLog("Simple error message", new NotImplementedException("Test Exception"));

        await Task.Delay(100);

        _logger.InformationLog(
            "Simple message 1 argument {Guid}!",
            Guid.NewGuid());

        await Task.Delay(100);

        _logger.ErrorLog("Simple error message 1 argument {Guid}",
            Guid.NewGuid(),
            new NotImplementedException("Test Exception"));

        await Task.Delay(100);
        _logger.WarningLog(arg0: 100, arg1: '?', "Initial two args were, int and char!");
    }
}

