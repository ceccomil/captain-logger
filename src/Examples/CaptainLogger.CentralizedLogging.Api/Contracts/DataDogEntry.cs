using System.Text.Json.Serialization;

namespace CaptainLogger.CentralizedLogging.Api.Contracts;

public record DataDogEntry(
    string Service,
    string HostName,
    string Level,
    [property: JsonPropertyName("ddsource")] string DdSource,
    [property: JsonPropertyName("ddtags")] string DdTags,
    object Message,
    string? STackTrace
    );