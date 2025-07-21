using BenchmarkDotNet.Attributes;
using CaptainLogger.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CaptainLogger.Benchmarks;

[MemoryDiagnoser]
public class Benchy
{
    private readonly ICaptainLogger _logger;

    public Benchy()
    {
        var sp = new ServiceCollection()
            .Configure<CaptainLoggerOptions>(x =>
            {
                x.ArgumentsCount = LogArguments.Two;
                x.Templates.Add(LogArguments.One, "Single value:\r\n{Val1}");
                x.Templates.Add(LogArguments.Two, "Two values:\r\n{Val1}\r\n{Val2}");
                x.LogRecipients = Recipients.None;
            })
        .AddLogging(builder =>
        {
            builder
            .ClearProviders()
            .AddCaptainLogger()
            .AddFilter(typeof(Benchy).Namespace, LogLevel.Information);
        })
        .AddScoped<ILoggingBenchmarks, LoggingBenchmarks>()
        .BuildServiceProvider()
        .CreateScope()
        .ServiceProvider
        ?? throw new InvalidOperationException();

        var lb = sp.GetRequiredService<ILoggingBenchmarks>();

        _logger = lb.Logger;
    }

    [Benchmark]
    public void DirectMessage() => _logger.InformationLog("This is just a message!");

    [Benchmark]
    public void DirectMessageNotEnabled() => _logger.DebugLog("This is just a message!");

    [Benchmark]
    public void StringInterpolation() => _logger.InformationLog(arg0: $"Logging a guid `{Guid.NewGuid}` using string interpolation");

    [Benchmark]
    public void AValueType() => _logger.InformationLog(100);

    [Benchmark]
    public void TwoValueTypes() => _logger.InformationLog(100, 200);

    [Benchmark]
    public void TwoValueTypes_NET6Gen() => Net6GenLogMessage.LogTwoValueTypes(_logger.RuntimeLogger, 100, 200);

    [Benchmark]
    public void StringInterpolationNotEnabled() => _logger.DebugLog(arg0: $"Logging a guid `{Guid.NewGuid}` using string interpolation");

    [Benchmark]
    public void AValueTypeNotEnabled() => _logger.DebugLog(100);

    [Benchmark]
    public void TwoValueTypesNotEnabled() => _logger.DebugLog(100, 200);
}

