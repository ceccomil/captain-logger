
using CaptainLogger.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CaptainLogger.Benchmarks;

public class LoggingBenchmarks : ILoggingBenchmarks
{
    public ICaptainLogger Logger { get; }
    public LoggingBenchmarks(ICaptainLogger<LoggingBenchmarks> logger)
    {
        Logger = logger;
    }
}
