namespace CaptainLogger.RequestTracer.Headers;

internal class CorrelationHandler : ICorrelationHandler
{
    private readonly IHttpContextAccessor _contextAccessor;

    public CorrelationHandler(
        IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public void Append(
        HttpClient client)
    {
        if (_contextAccessor.HttpContext is null)
        {
            throw new NotSupportedException(
                $"A valid {nameof(HttpContext)} is required");
        }

        var traceId = _contextAccessor
            .HttpContext
            .TraceIdentifier;

        if (client
            .DefaultRequestHeaders
            .Any(x => x
                .Key
                .Equals(
                    CorrelationHeader,
                    StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        client
            .DefaultRequestHeaders
            .Add(CorrelationHeader, traceId);
    }
}
