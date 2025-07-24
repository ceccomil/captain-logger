using CaptainLogger;
using CaptainLogger.Contracts.EventsArgs;

namespace VerySimpleUseCase;

public interface ILoggerReceiver
{

}

internal sealed class LoggerReceiver : ILoggerReceiver
{
  public LoggerReceiver(
    ICaptainLogger logger)
  {
    logger.LogEntryRequestedAsync += OnLogEntry;
  }

  private Task OnLogEntry(CaptainLoggerEventArgs<object> evArgs)
  {
    var stop = "stop";
    return Task.CompletedTask;
  }
}
