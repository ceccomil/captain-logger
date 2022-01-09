using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CaptainLogger.Tests.ConsoleApp;

public static class TestEnvironment
{
    public static async Task Scope(
        Func<IServiceProvider, Task> test,
        Func<IServiceCollection, IServiceCollection>? addServices = default)
    {
        var services = SetServices();

        if (addServices is not null)
            services = addServices(services);

        using var sp = services
            .BuildServiceProvider(true);

        using var scope = sp.CreateScope();

        await test(scope.ServiceProvider);
    }

    private static IServiceCollection SetServices() => new ServiceCollection()
        .AddLogging(builder =>
        {
            builder
                .ClearProviders()
                .AddCaptainLogger()
                .AddFilter("System", LogLevel.Trace)
                .AddFilter("Microsoft", LogLevel.Trace)
                .AddFilter(typeof(TestEnvironment).Namespace, LogLevel.Information);
        });
}
