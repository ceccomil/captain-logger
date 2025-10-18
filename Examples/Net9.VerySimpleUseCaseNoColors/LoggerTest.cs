using CaptainLogger;
using CaptainLogger.Generated;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace VerySimpleUseCase;

internal sealed class LoggerTest(
  ILogger<LoggerTest> _logger,
  IHttpClientFactory _httpFactory)
  : BackgroundService
{
  private static long _counter = 1;
  private static readonly Random _rng = new();

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await Task.Delay(1000, stoppingToken)
      .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

    _logger.InformationLog("Starting LoggerTest...");

    while (!stoppingToken.IsCancellationRequested)
    {
      // Don't wait to simulate some parrallel execution.
      await ScopedExecution(stoppingToken);

      await Task
        .Delay(1000)
        .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

      try
      {
        throw new InvalidOperationException(
          "This is a test exception to demonstrate error logging.");
      }
      catch (Exception ex)
      {
        _logger.ErrorLog(
          "An error occurred while executing the LoggerTest.",
          ex);

        throw;
      }

      _counter++;
    }
  }

  private async Task ScopedExecution(CancellationToken cancellationToken)
  {
    CaptainLoggerCorrelationScope.BeginScope(Guid.NewGuid());

    _logger.InformationLog(
      "Logging some info on the scope start...");

    await Task
      .Delay(_rng.Next(2500, 5000), cancellationToken)
      .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

    await AnotherMethod(cancellationToken);
  }

  private async Task AnotherMethod(CancellationToken cancellationToken)
  {
    _logger.WarningLog(
      "Logging something else from another method!");

    var client = _httpFactory.CreateClient("Test");

    var response = await client.GetAsync("https://www.google.com", cancellationToken);

    _logger.InformationLog(
      $"Got response from Google: {response.StatusCode}");

    _logger.WarningLog(10, 20, 30);
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
