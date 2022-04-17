namespace CaptainLogger.RequestTracer.Headers;

/// <summary>
/// A Service injected to help send the right trace identifier with subsequent requests.
/// </summary>
public interface ICorrelationHeader
{
    /// <summary>
    /// Appends the current trace identifier to the collection of request headers.
    /// </summary>
    /// <param name="client"></param>
    void Append(HttpClient client);
}
