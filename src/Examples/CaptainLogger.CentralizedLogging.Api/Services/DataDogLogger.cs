using CaptainLogger.CentralizedLogging.Api.Contracts;
using CaptainLogger.Contracts.EventArguments;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CaptainLogger.CentralizedLogging.Api.Services;

public class DataDogLogger : IDataDogLogger
{
    private readonly ICaptainLogger _logger;
    private readonly HttpClient _http;
    private static readonly string _hostName = Environment.MachineName;

    private const string SERVICE = "CentralizedLogging.API";

    // To test this a valid DataDog account is required
    // https://docs.datadoghq.com/api/latest/logs/#send-logs
    // DD_ENDPOINT and API KEY must be valid (accordingly to the instruction above)
    // !! make sure the URL is for the region you want!
    private const string DD_ENDPOINT = " https://http-intake.logs.datadoghq.com/api/v2/logs";
    private const string DD_API_KEY = "<my api key>";

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public bool EventListenerIsAttached { get; private set; }

    public DataDogLogger(
        ICaptainLogger<DataDogLogger> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _http = httpClientFactory
            .CreateClient("DD_LOGS");
    }

    private async Task LogEntryRequestedAsync(CaptainLoggerEvArgs<object> evArgs)
    {
        string? stackTrace = null;

        if (evArgs.Exception is not null)
            stackTrace = $"{evArgs.Exception}";

        var ddEntry = new DataDogEntry(
            SERVICE,
            _hostName,
            $"{evArgs.LogLevel}",
            evArgs.LogCategory,
            $"timeStamp:{evArgs.LogTime:yyyy-MM-ddTHH:mm:ss.fff},eventId{evArgs.LogEvent}",
            evArgs.State,
            stackTrace
            );

        using var req = new HttpRequestMessage(HttpMethod.Post, DD_ENDPOINT)
        {
            Headers =
            {
                { HeaderNames.Accept, "application/json" },
                { "DD-API-KEY", DD_API_KEY }
            },
            Content = new StringContent(JsonSerializer.Serialize(ddEntry, _jsonOpts))
        };

        using var resp = await _http.SendAsync(req);
        
        if (!resp.IsSuccessStatusCode)
        {
            //Do nothing!
            //Error handling on failures is out of the scope of this example!
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogEntryRequestedAsync += LogEntryRequestedAsync;
        EventListenerIsAttached = true;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogEntryRequestedAsync -= LogEntryRequestedAsync;
        EventListenerIsAttached = false;
        return Task.CompletedTask;
    }
}