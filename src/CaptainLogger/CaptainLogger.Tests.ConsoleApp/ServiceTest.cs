namespace CaptainLogger.Tests.ConsoleApp;

public class ServiceTest : IServiceTest
{
    private readonly ILogger _logger;
    private readonly Random _rng;
    private int _iteration = 1;

    public string InstanceId { get; }

    public ServiceTest(
        ILogger<ServiceTest> logger,
        IOptions<ServiceTestOptions> opts)
    {
        _logger = logger;
        _rng = new Random();
        InstanceId = opts.Value.InstanceId;
    }

    public async Task RunAsync()
    {
        var end = DateTime.UtcNow + TimeSpan.FromSeconds(2.0d);

        while (end > DateTime.UtcNow)
            await RandomLogs();
    }

    private async Task RandomLogs()
    {
        var iteration = $"{_iteration:000}";
        _iteration++;
        _logger.LogInformation("[{Iteration}] Instance {InstanceId}", iteration, InstanceId);
        await Task.Delay(_rng.Next(10, 100));
    }
}
