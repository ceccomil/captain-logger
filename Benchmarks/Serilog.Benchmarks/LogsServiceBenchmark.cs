using BenchmarkDotNet.Attributes;
using CaptainLogger.Contracts.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace LogsBenchmark;

[MemoryDiagnoser]
public class LogsServiceBenchmark
{
  private ILogsService _logsService = null!;

  [GlobalSetup]
  public void Setup()
  {
    var levelSwitch = new LoggingLevelSwitch();

    var serilogLogger = new LoggerConfiguration()
      .MinimumLevel.ControlledBy(levelSwitch)
      .WriteTo.Console(
          outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
      .WriteTo.File(
          path: Constants.LOG_FILE,
          outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
          rollingInterval: RollingInterval.Infinite,
          shared: true,
          buffered: false,
          flushToDiskInterval: TimeSpan.FromMilliseconds(1))
      .CreateLogger();

    var builder = new HostApplicationBuilder();

    builder
      .Logging
      .ClearProviders()
      .AddSerilog(serilogLogger, dispose: true)
      .AddFilter("", LogLevel.Information);

    builder
      .Services
      .Configure<CaptainLoggerOptions>(x =>
      {
        // Configure ILogger extensions even if the provider is not CaptainLogger
        x.ArgumentsCount = LogArguments.Three;
        x.Templates.Add(LogArguments.One, "This is a test log with one argument: {logValue1}");
        x.Templates.Add(LogArguments.Two, "This is a test log with two arguments: {logValue1} - {logValue2}");
        x.Templates.Add(LogArguments.Three, "This is a test log with 3 arguments: {logValue1}, {logValue2}, {logValue3}");
      })
      .AddSingleton<ILogsService, LogsService>();

    var host = builder.Build();
    _logsService = host.Services.GetRequiredService<ILogsService>();
  }

  [Benchmark]
  public void LogsInParallel()
  {
    Parallel.For(0, 1000, i => _logsService.Logs(i));
  }

  [Benchmark]
  public void OneLogLine()
  {
    _logsService.OneLogLine();
  }

  [Benchmark]
  public void OneLogLineDisabled()
  {
    _logsService.OneLogLineDisabled();
  }
}
