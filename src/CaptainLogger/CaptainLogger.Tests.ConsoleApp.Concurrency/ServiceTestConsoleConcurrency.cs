namespace CaptainLogger.Tests.ConsoleApp.Concurrency;

public class ServiceTestConsoleConcurrency : IServiceTest
{
    private readonly ILogger _logger;

    public string InstanceId { get; }

    public ServiceTestConsoleConcurrency(
        ILogger<ServiceTest> logger,
        IOptions<ServiceTestOptions> opts)
    {
        _logger = logger;
        InstanceId = opts.Value.InstanceId;
    }

    public Task RunAsync()
    {
        var result = Parallel.For(0, 2000, i => LogLine(i));

        return Task.CompletedTask;
    }

    private void LogLine(int i)
    {
        var time = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}]";
        var index = $"[{i:0000}]";
        var guid = $"[{Guid.NewGuid()}]";
        _logger
            .LogInformation(
                "{Index} - {Guid} - {Time}",
                index,
                guid,
                time);

    }
}
