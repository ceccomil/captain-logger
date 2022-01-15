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
            .WarningLog("I am logging a new guid", arg1: guid);

        var log = Encoding.UTF8.GetString(reader.ToArray());
        Assert.Contains($"I am logging a new guid", log);
        Assert.Contains($"{guid}", log);

        reader.SetLength(0);

        await Task.Delay(100);

        var n = new Random().Next(0, 1000);

        writer
            .InformationLog(typeof(Random).Name, "Next(0, 1000)", n);

        writer
            .ErrorLog("Test exception", new Exception("Exception"));

        log = Encoding.UTF8.GetString(reader.ToArray());
        Assert.DoesNotContain($"This is a log for", log);

        writer
            .WarningLog(typeof(Random).Name, "Next(0, 1000)", n);

        log = Encoding.UTF8.GetString(reader.ToArray());
        Assert.Contains($"This is a log for", log);
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
                opts.ArgumentsCount = LogArguments.Three;
                opts.Templates.Add(LogArguments.Three, "This is a log for `{Class}`, `{Method}` which resulted in {Result}");
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
