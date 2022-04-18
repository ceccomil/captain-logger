namespace CaptainLogger.RequestTracer.Tests;

public class CorrelationMiddlewareTests
{
    private readonly HttpContext _httpContext = Substitute.For<HttpContext>();
    private readonly RequestDelegate _reqDelegate = Substitute.For<RequestDelegate>();

    [Fact(DisplayName = "TraceIdentifier won't be assigned if no header is present")]
    public async Task NoHeaders()
    {
        // Arrange
        var middleware = new CorrelationMiddleware();

        // Act
        await middleware.InvokeAsync(_httpContext, _reqDelegate);

        // Assert
        _httpContext
            .Request
            .Headers
            .Should()
            .BeEmpty();

        string
            .IsNullOrEmpty(_httpContext.TraceIdentifier)
            .Should()
            .BeTrue();
    }

    [Theory(DisplayName = "TraceIdentifier will be assigned if header is passed")]
    [InlineData(1)]
    [InlineData(2)]
    public async Task WithHeader(int count)
    {
        // Arrange
        var middleware = new CorrelationMiddleware();

        var traceId = Guid
            .NewGuid()
            .ToString();

        var dict = new Dictionary<string, StringValues>()
        {
            { "Content-Type", "application/json" },
            { CorrelationHeader,  GetValues(count, traceId) }
        };

        _httpContext
            .Request
            .Headers
            .Returns(new HeaderDictionary(dict));

        // Act
        await middleware
            .InvokeAsync(_httpContext, _reqDelegate);

        // Assert
        _httpContext
            .Request
            .Headers
            .Count
            .Should()
            .Be(2);

        var check = "";
        for (var i = 0; i < count; i++)
        {
            check += traceId + "-";
        }

        if (check.Length > 0)
        {
            check = check.Remove(check.Length - 1);
        }

        _httpContext
            .TraceIdentifier
            .Should()
            .Be(check);
    }

    private static StringValues GetValues(int count, string value)
    {
        var values = new string[count];
        for (var i = 0; i < count; i++)
        {
            values[i] = value;
        }

        return new(values);
    }
}
