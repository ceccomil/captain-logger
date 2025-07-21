namespace CaptainLogger.CentralizedLogging.Api.Services;

public interface IDataDogLogger : IHostedService
{
    bool EventListenerIsAttached { get; }
}
