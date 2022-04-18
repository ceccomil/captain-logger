namespace CaptainLogger.Tests.ConsoleApp;

public class ServiceTest : IServiceTest
{
    private readonly ILogger _logger;
    private readonly Random _rng;
    private int _iteration = 1;

    public string InstanceId { get; }

    public ServiceTest(
        ILogger<ServiceTest> logger,
        IOptions<ServiceTestOptions> opts,
        IServiceCollection services)
    {
        _logger = logger;
        _rng = new Random();
        InstanceId = opts.Value.InstanceId;

        foreach (var service in services)
        {
            _logger
                .LogDebug("Injected service type: {ServiceType}, implementation type: {ImplementationType}",
                service.ServiceType.Name,
                service.ImplementationType?.Name
                ?? "implementation type is null");
        }
    }

    public async Task RunAsync()
    {
        var end = DateTime.UtcNow + TimeSpan.FromSeconds(2.0d);

        while (end > DateTime.UtcNow)
        {
            await RandomLogs();
        }
    }

    private async Task RandomLogs()
    {
        var rng = _rng.Next(1, 4);

        var additionalMex = "";
        if (rng == 1)
        {
            var json = JsonSerializer.Serialize(
            new
            {
                TestValue1 = rng,
                TestValue2 = "TestValue2",
                TestValue3 = DateTime.Now
            },
            new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                WriteIndented = true
            });

            additionalMex = $"{Environment.NewLine}Json:{Environment.NewLine}{json}";
        }

        Exception? ex = null;
        if (rng == 3)
        {
            try
            {
                throw new ApplicationException("ApplicationException Example Message!");
            }
            catch (Exception ex1)
            {
                try
                {
                    throw new NotImplementedException("NotImplementedException!", ex1);
                }
                catch (Exception ex2)
                {
                    ex = ex2;
                }
            }
        }

        var iteration = $"{_iteration:000}";
        _iteration++;

        if (ex is null)
        {
            _logger
                .LogInformation(
                "[{Iteration}] Instance {InstanceId}{AdditionalMex}",
                iteration,
                InstanceId,
                additionalMex);
        }
        else
        {
            _logger
                .LogError(
                ex,
                "[{Iteration}] Instance {InstanceId}{AdditionalMex}",
                iteration,
                InstanceId,
                additionalMex);
        }

        await Task.Delay(_rng.Next(10, 100));
    }
}
