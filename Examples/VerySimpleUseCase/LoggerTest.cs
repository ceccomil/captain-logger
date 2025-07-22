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

      //_cptLogger.InformationLog(
      //  """
      //  This is a CaptainLogger log message.
      //  With a json:
      //  {
      //    "Name": "CaptainLogger",
      //    "Version": "1.0.0"
      //  }
      //  """);

      _cptLogger.InformationLog("This is a CaptainLogger log message.");
      _iLogger.LogInformation("This is an ILogger log message.");

      _cptLogger.TempInfo(10, 20);

      _cptLogger.TempInfo(DateTime.Now, Guid.NewGuid());

      _cptLogger.TempInfo(
        "This is a temporary info log message.",
        new { Name = "CaptainLogger", Version = "1.0.0" });

      var test = new Test
      {
        Name = "CaptainLogger",
        Version = "1.0.0"
      };

      _cptLogger.TempInfo(
        DateTimeOffset.Now,
        test);

      List<byte> testBytes1 =
      [
        10, 20, 30, 40, 50, 60, 70, 80, 90, 100
      ];

      byte[] testBytes2 =
      [
        10, 20, 30, 40, 50, 60, 70, 80, 90, 100
      ];

      List<TestEnum> testList =
      [
        TestEnum.Zero,
        TestEnum.One,
        TestEnum.Two
      ];

      _cptLogger.TempInfo(
        80,
        testBytes2);
    }
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
