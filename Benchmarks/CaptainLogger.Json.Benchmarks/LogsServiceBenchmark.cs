using BenchmarkDotNet.Attributes;
using CaptainLogger;
using CaptainLogger.Contracts.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LogsBenchmark;

[MemoryDiagnoser]
public class LogsServiceBenchmark
{
  private ILogsService _logsService = null!;

  [GlobalSetup]
  public void Setup()
  {
    var builder = new HostApplicationBuilder();

    builder
      .Logging
      .ClearProviders()
      .AddCaptainLogger()
      .AddFilter("", LogLevel.Information);

    builder
      .Services
      .Configure<CaptainLoggerOptions>(x =>
      {
        x.HighPerfStructuredLogging = true;
        x.TimeIsUtc = true;
        x.FilePath = Constants.LOG_FILE;
        x.FileRotation = LogRotation.None;
        x.LogRecipients = Recipients.Console | Recipients.File;

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
