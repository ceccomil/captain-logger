namespace CaptainLogger.Tests.ConsoleApp.Generator;

public static class Program
{
    private static string _appId = null!;

    public static async Task Main(string[] args)
    {
        if (!args.Any())
            _appId = Guid.NewGuid().ToString();
        else
            _appId = args[0];

        await EnvInit(Run);

        static async Task Run(IServiceProvider sp)
        {
            var service = sp.GetRequiredService<IServiceTest>();
            await service.RunAsync();
        }
    }

    private static async Task EnvInit(Func<IServiceProvider, Task> runTest)
    {
        await TestEnvironment.Scope(runTest, AddConcurrentService);

        static IServiceCollection AddConcurrentService(IServiceCollection s) => s
            .Configure<ServiceTestOptions>(x => x.InstanceId = _appId)
            .Configure<CaptainLoggerOptions>(x =>
            {
                x.TimeIsUtc = true;
                x.ArgumentsCount = LogArguments.Seven;
            })
            .AddScoped<IServiceTest, ServiceTestConsoleGenerator>();
    }
}