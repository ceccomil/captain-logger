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
      _cptLogger.InformationLog("This is a CaptainLogger log message.");
      _iLogger.LogInformation("This is an ILogger log message.");
      await Task.Delay(3000, stoppingToken);
    }
  }
}
