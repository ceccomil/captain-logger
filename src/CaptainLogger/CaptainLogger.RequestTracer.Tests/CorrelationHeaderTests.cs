namespace CaptainLogger.RequestTracer.Tests;

public class CorrelationHeaderTests
{
    private readonly IHttpContextAccessor _contextAccessor = Substitute.For<IHttpContextAccessor>();
    private readonly HttpContext _context = Substitute.For<HttpContext>();

    private readonly Fixture _fixture = new();

    private const string TRACE_ID = "AEB37AAC-D5BE-4BD7-89DF-0718F5FD2BBC";

    public CorrelationHeaderTests()
    {
        _context.TraceIdentifier.Returns(TRACE_ID);
        _contextAccessor.HttpContext.Returns(_context);
    }

    [Fact(DisplayName = "Trace identifier is added to client headers")]
    public void AddHeader()
    {
        // Arrange
        var svc = new CorrelationHandler(_contextAccessor);
        
        var client = _fixture.Create<HttpClient>();

        // Act
        svc.Append(client);

        // Assert
        client
            .DefaultRequestHeaders
            .Contains(CorrelationHeader)
            .Should()
            .BeTrue();

        client
            .DefaultRequestHeaders
            .GetValues(CorrelationHeader)
            .Single()
            .Should()
            .Be(TRACE_ID);
    }

    [Fact(DisplayName = "Trace identifier is not added if already there")]
    public void AddHeaderWhenThere()
    {
        // Arrange
        var svc = new CorrelationHandler(_contextAccessor);

        var client = _fixture.Create<HttpClient>();
        client
            .DefaultRequestHeaders
            .Add(CorrelationHeader, TRACE_ID);

        // Act
        svc.Append(client);

        // Assert
        client
            .DefaultRequestHeaders
            .Count()
            .Should()
            .Be(1);

        client
            .DefaultRequestHeaders
            .Contains(CorrelationHeader)
            .Should()
            .BeTrue();

        client
            .DefaultRequestHeaders
            .GetValues(CorrelationHeader)
            .Single()
            .Should()
            .Be(TRACE_ID);
    }
}
