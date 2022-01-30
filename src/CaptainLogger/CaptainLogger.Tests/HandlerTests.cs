using Microsoft.Extensions.Options;

namespace CaptainLogger.Tests;

public class HandlerTests
{
    private static readonly IFixture _fixture = new Fixture();
    private readonly CaptainLoggerOptions _options = _fixture
        .Build<CaptainLoggerOptions>()
        .With(x => x.LoggerBuffer, new MemoryStream())
        .With(x => x.LogRecipients, Recipients.Console)
        .With(x => x.TriggerEvents, true)
        .Create();

    private readonly IOptionsMonitor<CaptainLoggerOptions> _optMon = Substitute
        .For<IOptionsMonitor<CaptainLoggerOptions>>();

    private ICaptainLogger Setup()
    {
        _optMon.CurrentValue.Returns(_options);

        var loggerProvider = new CaptainLoggerProvider(_optMon);

        loggerProvider
            .CreateLogger($"{typeof(HandlerTests).Namespace}.{typeof(HandlerTests).Name}");

        ICaptainLogger logger = new CaptainLoggerBase<HandlerTests>(
            loggerProvider.Loggers.First().Value,
            loggerProvider);

        return logger;
    }

    [Fact(DisplayName = "Events are triggered on log entry requests (sync)")]
    public void EventHandlerTest()
    {
        //Arrange
        object? handledMex = null;
        var myMex = "Simple log message!";

        var logger = Setup();

        void LogEntryRequested(CaptainLoggerEvArgs<object> evArgs)
        {
            //Some long operation
            Thread.Sleep(250);

            handledMex = evArgs.State;
        }

        logger.LogEntryRequested += LogEntryRequested;

        //Act
        logger.InformationLog(myMex);

        logger.LogEntryRequested -= LogEntryRequested;

        //Assert
        handledMex.Should().NotBeNull();
        $"{handledMex}".Should().Be(myMex);
    }

    [Fact(DisplayName = "Events are triggered on log entry requests (async)")]
    public async Task EventHandlerTestAsync()
    {
        //Arrange
        object? handledMex = null;
        var myMex = "Simple log message!";

        var logger = Setup();

        async Task LogEntryRequested(CaptainLoggerEvArgs<object> evArgs)
        {
            //Some long operation
            await Task.Delay(250);

            handledMex = evArgs.State;

            throw new ApplicationException("Test won't fail! This will be ignored!");
        }

        logger.LogEntryRequestedAsync += LogEntryRequested;

        //Act
        logger.InformationLog(myMex);

        await Task.Delay(500);

        logger.LogEntryRequestedAsync -= LogEntryRequested;

        //Assert
        handledMex.Should().NotBeNull();
        $"{handledMex}".Should().Be(myMex);
    }
}
