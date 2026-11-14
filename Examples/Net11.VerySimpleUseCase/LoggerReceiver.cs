using CaptainLogger;
using CaptainLogger.Contracts.EventsArgs;

namespace VerySimpleUseCase;

public interface ILoggerReceiver
{

}

internal sealed class LoggerReceiver : ILoggerReceiver
{
  public LoggerReceiver(
    ILogDispatcher dispatcher)
  {
    dispatcher.OnLogEntry += OnLogEntry;
  }

  private Task OnLogEntry(CaptainLoggerEventArgs<object> evArgs)
  {
    var stop = "stop";
    return Task.CompletedTask;
  }
}
