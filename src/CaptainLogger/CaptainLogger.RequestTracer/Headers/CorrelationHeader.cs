namespace CaptainLogger.RequestTracer.Headers;

internal class CorrelationHeader : ICorrelationHeader
{
    private readonly IHttpContextAccessor _contextAccessor;

    public CorrelationHeader(
        IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public void AppendCorrelationHeader(HttpClient client)
    {
        var traceId = _contextAccessor
            .HttpContext
            .TraceIdentifier;

        if (client.DefaultRequestHeaders.Any(x => x.Key.Equals(CORRELATION_HEADER, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        client
            .DefaultRequestHeaders
            .Add(CORRELATION_HEADER, traceId);
    }
}
