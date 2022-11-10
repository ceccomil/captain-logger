using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaptainLogger.CentralizedLogging.Api.Contracts;

public class LogEntry
{
    public string? Message { get; set; }
    public string? TraceId { get; set; }
    public Guid? CorrelationId { get; set; }
    public string? SourceMethod { get; set; }
    public string? Host { get; set; }
    public string? Env { get; set; }
    public object? ExtraContent { get; set; }
    public IDictionary<string, object> Tags => GetTags();

    public override string ToString() => Message ?? "";

    private IDictionary<string, object> GetTags()
    {
        var dict = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(Message))
        {
            dict.Add("message", Message);
        }

        if (!string.IsNullOrWhiteSpace(TraceId))
        {
            dict.Add("traceId", TraceId);
        }

        if (CorrelationId is not null &&
            CorrelationId != Guid.Empty)
        {
            dict.Add("correlationId", CorrelationId);
        }

        if (!string.IsNullOrWhiteSpace(SourceMethod))
        {
            dict.Add("sourceMethod", SourceMethod);
        }

        if (!string.IsNullOrWhiteSpace(Host))
        {
            dict.Add("host", Host);
        }

        if (!string.IsNullOrWhiteSpace(Env))
        {
            dict.Add("env", Env);
        }

        if (ExtraContent is not null)
        {
            dict.Add("extraContent", ExtraContent);
        }

        return dict;
    }
}
