using CaptainLogger;
using CaptainLogger.Contracts;
using Microsoft.Extensions.Hosting;

namespace VerySimpleUseCase;

internal sealed class LoggerTest(
  ICaptainLogger<LoggerTest> _cptLogger)
  : BackgroundService
{
  private static long _counter = 1;
  private static readonly Random _rng = new();

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await Task.Delay(1000, stoppingToken)
      .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

    _cptLogger.InformationLog("Starting LoggerTest...");

    while (!stoppingToken.IsCancellationRequested)
    {
      // Don't wait to simulate some parrallel execution.
      _ = ScopedExecution(stoppingToken);

      await Task
        .Delay(1000)
        .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

      _counter++;
    }
  }

  private async Task ScopedExecution(CancellationToken cancellationToken)
  {
    CaptainLoggerCorrelationScope.BeginScope(Guid.NewGuid());

    _cptLogger.InformationLog(
      "Logging some info on the scope start...");

    await Task
      .Delay(_rng.Next(2500, 5000), cancellationToken)
      .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

    await AnotherMethod(cancellationToken);
  }

  private async Task AnotherMethod(CancellationToken cancellationToken)
  {
    _cptLogger.WarningLog(
      "Logging something else from another method!");

    await Task
      .Delay(250, cancellationToken)
      .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
  }
}

internal class Test
{
  public required string Name { get; init; }
  public required string Version { get; init; }

  public override string ToString()
  {
    return $"{Name} - {Version}";
  }
}

internal enum TestEnum
{
  Zero,
  One,
  Two,
  Three
}
