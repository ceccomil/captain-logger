using CaptainLogger.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace VerySimpleUseCase;

internal sealed class LoggerTest(
  ICaptainLogger<LoggerTest> _cptLogger,
  ILogger<LoggerTest> _iLogger) : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      await Task
        .Delay(3000, stoppingToken)
        .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

      _cptLogger.InformationLog(
        """
        This is a CaptainLogger log message.
        With a json:
        {
          "Name": "CaptainLogger",
          "Version": "1.0.0"
        }
        """);
      _iLogger.LogInformation("This is an ILogger log message.");

      _cptLogger.TempInfo(10, 20);

      _cptLogger.TempInfo(DateTime.Now, Guid.NewGuid());
    }
  }
}
