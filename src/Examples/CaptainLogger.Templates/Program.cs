namespace CaptainLogger.Templates;

public static class Program
{
    public static void Main()
    {
        var service = SetupAndGetService();
        service
            .LogWebRequests("10.0.0.2", 443, 204, "OPTIONS");
        
        service
            .LogUserIds(10, 1025, 1);

        service
            .LogWebRequests("172.23.34.19", 80, 503, "POST");

        service
            .LogUserIds(115, 2850, 2);

        service
            .BaseLogger
            .LogInformation(
                12,
                "Something from base logger not filtered");

        service
            .BaseLogger
            .LogInformation(
                13,
                "Something from base logger filtered");

        service
            .BaseLogger
            .LogInformation(
                14,
                "Something from base logger filtered");
    }

    private static ILoggingExample SetupAndGetService()
    {
        using var sp = new ServiceCollection()
            .Configure<CaptainLoggerOptions>(opts =>
            {
                opts.TimeIsUtc = true;
                opts.ArgumentsCount = LogArguments.Four;
                opts.Templates.Add(LogArguments.Three, "Request subbmitted by user id {UserId}, of department [{DepId}] - client id {ClientId}");
                opts.Templates.Add(LogArguments.Four, "Method [{Method}] - URL: {Hostname}:{Port} - status code returned: {StatusCode}");
                opts.LogRecipients = Recipients.Console | Recipients.File;
                opts.ExcludedEventIds = new int[2] { 13, 14 };
            })
            .AddLogging(builder =>
            {
                builder
                    .ClearProviders()
                    .AddCaptainLogger()
                    .AddFilter(typeof(Program).Namespace, LogLevel.Information);
            })
            .AddSingleton<ILoggingExample, LoggingExample>()
            .BuildServiceProvider();

        return sp.GetRequiredService<ILoggingExample>();
    }
}