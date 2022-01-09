namespace CaptainLogger.Tests.ConsoleApp;

public interface IServiceTest
{
    string InstanceId { get; }
    Task RunAsync();
}
