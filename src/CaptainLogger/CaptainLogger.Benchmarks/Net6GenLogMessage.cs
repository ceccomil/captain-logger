using Microsoft.Extensions.Logging;


namespace CaptainLogger.Benchmarks;

public partial class Net6GenLogMessage
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Test {Arg0}, {Arg1}")]
    public static partial void LogTwoValueTypes(ILogger loger, int arg0, int arg1);
}
