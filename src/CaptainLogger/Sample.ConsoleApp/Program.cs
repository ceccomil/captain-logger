using CaptainLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sample.ConsoleApp;

static class Program
{
    static async Task Main()
    {
        using var sp = SetServices()
            .BuildServiceProvider(true);
        
        using var scope = sp.CreateScope();

        var service = scope
            .ServiceProvider
            .GetRequiredService<IServiceSample>();

        await service.RunAsync();
    }

    static IServiceCollection SetServices() => new ServiceCollection()
        .AddLogging(builder =>
        {
            builder
                .ClearProviders()
                .AddCaptainLogger()
                .AddFilter("System", LogLevel.Error)
                .AddFilter("Microsoft", LogLevel.Error)
                .AddFilter(typeof(Program).Namespace, LogLevel.Trace);
        })
        .AddScoped<IServiceSample, ServiceSample>();
}
