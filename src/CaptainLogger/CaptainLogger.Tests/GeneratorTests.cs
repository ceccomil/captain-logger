using CaptainLogger.Options;
using Microsoft.Extensions.Options;

namespace CaptainLogger.Tests;

public class GeneratorTests
{
    [Fact(DisplayName = "Generator is providing the exention method required!")]
    public async Task GeneratorIsWorking()
    {
        using var reader = new MemoryStream();
        var writer = GetWriter(reader);
        await Task.Delay(100);

        var guid = Guid.NewGuid();

        writer
            .WarningLog("I am logging a new guid {Guid}", arg0: guid);

        var log = Encoding.UTF8.GetString(reader.ToArray());
        Assert.Contains($"I am logging a new guid {guid}", log);

        reader.SetLength(0);

        await Task.Delay(100);

        var n = new Random().Next(0, 1000);

        writer
            .InformationLog("I am logging a random int {Number}", arg0: n);

        writer
            .ErrorLog("Test exception", new Exception("Exception"));

        log = Encoding.UTF8.GetString(reader.ToArray());
        Assert.DoesNotContain($"I am logging a random int {n}", log);
    }

    private class TestService
    {
        public ICaptainLogger Writer { get; set; }
        public TestService(
            ICaptainLogger<TestService> logger)
        {
            Writer = logger;
        }
    }

    private static IServiceCollection GetServices(Stream reader)
    {
        var services = new ServiceCollection()
            .AddLogging(builder =>
            {
                builder
                    .ClearProviders()
                    .AddCaptainLogger()
                    .AddFilter(typeof(GeneratorTests).Assembly.GetName().Name, LogLevel.Warning);
            })
            .Configure<CaptainLoggerOptions>(opts =>
            {
                opts.ArgumentsCount = LogArguments.One;
                opts.LogRecipients = Recipients.Stream;
                opts.LoggerBuffer = reader;
            })
            .AddSingleton<TestService>();

        return services;
    }

    private static ICaptainLogger GetWriter(Stream reader)
    {
        using var sp = GetServices(reader).BuildServiceProvider();

        var service = sp.GetRequiredService<TestService>();

        return service.Writer;
    }
}
