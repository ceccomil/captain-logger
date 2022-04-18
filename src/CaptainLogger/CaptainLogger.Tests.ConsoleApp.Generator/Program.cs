namespace CaptainLogger.Tests.ConsoleApp.Generator;

public static class Program
{
    private static string _appId = null!;

    public static async Task Main(string[] args)
    {
        _appId = Guid.NewGuid().ToString();

        if (args.Any())
        {
            _appId = args[0];
        }
        
        await EnvInit(Run);

        static async Task Run(IServiceProvider sp)
        {
            var service = sp.GetRequiredService<IServiceTest>();
            await service.RunAsync();
        }
    }

    private static async Task EnvInit(Func<IServiceProvider, Task> runTest)
    {
        await TestEnvironment.Scope(runTest, AddService);

        static IServiceCollection AddService(IServiceCollection s) => s
            .Configure<ServiceTestOptions>(x => x.InstanceId = _appId)
            .Configure<CaptainLoggerOptions>(x =>
            {
                x.TimeIsUtc = true;
                x.ArgumentsCount = LogArguments.Three;
                //x.Templates.Add(LogArguments.One, "The value: {Arg0}" + Environment.NewLine + "Example log template");
                x.Templates.Add(LogArguments.Two, "Simple mex for two values ({Val1}, {Val2})");
            })
            .AddScoped<IServiceTest, ServiceTestConsoleGenerator>();
    }
}