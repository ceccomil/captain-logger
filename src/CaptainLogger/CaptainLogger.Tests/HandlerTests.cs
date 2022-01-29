namespace CaptainLogger.Tests;

public class HandlerTests
{
    private static IFixture _fixture = new Fixture();
    private readonly IServiceProvider _sp = Substitute.For<IServiceProvider>();
    private readonly CaptainLoggerOptions _options = _fixture
        .Build<CaptainLoggerOptions>()
        .With(x => x.LoggerBuffer, new MemoryStream())
        .Create();

    [Fact(DisplayName = "One injected handler test")]
    public void OneHandler()
    {
        //Arrange
        var handler = Substitute.For<ICaptainLoggerHandler>();
        _sp
            .TryGetServices<ICaptainLoggerHandler>()
            .Returns(new[] {handler});

        var cptLogger = new CptLogger(
            nameof(HandlerTests),
            () => _options,
            _sp);

        //Act
        cptLogger
            .Log(
                LogLevel.Information,
                0,
                "Simple message",
                exception: null,
                formatter: (state, ex) => state
            );

        //Assert
        handler
            .Received(1)
            .LogEntryRequested(Arg.Any<CaptainLoggerEvArgs<string>>());
    }

    [Fact(DisplayName = "Two injected handlers test")]
    public void TwoHandlers()
    {
        //Arrange
        var handler1 = Substitute.For<ICaptainLoggerHandler>();
        var handler2 = Substitute.For<ICaptainLoggerHandler>();

        _sp
            .TryGetServices<ICaptainLoggerHandler>()
            .Returns(new[] { handler1, handler2 });

        var cptLogger = new CptLogger(
            nameof(HandlerTests),
            () => _options,
            _sp);

        //Act
        cptLogger
            .Log(
                LogLevel.Information,
                0,
                "Message with execption",
                exception: new NotImplementedException(),
                formatter: (state, ex) => state
            );

        //Assert
        handler1
            .Received(1)
            .LogEntryRequested(Arg.Any<CaptainLoggerEvArgs<string>>());

        handler2
            .Received(1)
            .LogEntryRequested(Arg.Any<CaptainLoggerEvArgs<string>>());
    }
}
