namespace CaptainLogger.Templates.Services;

public interface ILoggingExample
{
    ILogger BaseLogger { get; }

    void LogUserIds(
        int userId,
        int departmentId,
        int clientId);

    void LogWebRequests(
        string hostname,
        int port,
        int statusCode,
        string method);
}
