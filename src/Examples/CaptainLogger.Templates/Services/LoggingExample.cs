namespace CaptainLogger.Templates.Services;

public class LoggingExample : ILoggingExample
{
    private readonly ICaptainLogger _logger;

    public LoggingExample(ICaptainLogger<LoggingExample> logger)
    {
        _logger = logger;
    }

    public void LogUserIds(
        int userId,
        int departmentId,
        int clientId) => _logger
        .InformationLog(
            userId,
            departmentId,
            clientId);

    public void LogWebRequests(
        string hostname,
        int port,
        int statusCode,
        string method)
    {
        if (statusCode >= 500)
        {
            _logger.WarningLog(method, hostname, port, statusCode);
        }

        if (statusCode >= 200 && statusCode < 300)
        {
            _logger.InformationLog(method, hostname, port, statusCode);
        }
    }
}
