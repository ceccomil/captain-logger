using CaptainLogger.Generated;
using Microsoft.Extensions.Logging;
using System.Net;

namespace LogsBenchmark;

public interface ILogsService
{
  void Logs(int index);
  void OneLogLine();
  void OneLogLineDisabled();
}

internal sealed class LogsService(
  ILogger<LogsService> _logger) : ILogsService
{
  private sealed record ExampleDto(
    string Name,
    string Surname,
    Gender Gender,
    int Age,
    decimal Score);

  private enum Gender
  {
    Female,
    Male
  }

  private static readonly ExampleDto _johnDoe = new("John", "Doe", Gender.Male, 30, 99.99M);
  private static readonly ExampleDto _janeDoe = new("John", "Doe", Gender.Female, 30, 99.99M);

  public void Logs(int index)
  {
    _logger.InformationLog(10, 20.5555M, true);

    _logger.InformationLog("This is a test log with no arguments (it's actually one string argument)");

    _logger.InformationLog(new { Name = "TestObject", Value = 42.4242424242M });

    _logger.InformationLog("Jane Doe", _janeDoe);

    _logger.TraceLog("John Doe", _johnDoe); // disabled

    _logger.DebugLog("This is a debug log (debug is disabled)");

    try
    {
      throw new InvalidOperationException("This is a test exception");
    }
    catch (Exception ex)
    {
      _logger.ErrorLog("An error occurred while processing the logs", ex);
    }

    _logger.WarningLog("Index", index);
  }

  public void OneLogLine()
  {
    byte[] bytes = [10, 20, 30, 40, 50, 60, 70, 80];
    _logger.CriticalLog(HttpMethod.Get, HttpStatusCode.Forbidden, bytes);
  }
  public void OneLogLineDisabled()
  {
    byte[] bytes = [10, 20, 30, 40, 50, 60, 70, 80];
    _logger.DebugLog(HttpMethod.Get, HttpStatusCode.Forbidden, bytes);
  }
}
