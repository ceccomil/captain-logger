using CaptainLogger.CentralizedLogging.Api.Contracts;
using CaptainLogger.Contracts.EventArguments;
using EazyHttp;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CaptainLogger.CentralizedLogging.Api.Services;

public class DataDogLogger : IDataDogLogger
{
    private readonly ICaptainLogger _logger;
    private readonly IEazyClients _clients;

    private readonly static string _hostName = Environment.MachineName;

    private const string SERVICE = "CentralizedLogging.API";

    public bool EventListenerIsAttached { get; private set; }

    public DataDogLogger(
        ICaptainLogger<DataDogLogger> logger,
        IEazyClients httpClients)
    {
        _logger = logger;
        _clients = httpClients;
    }

    private async Task LogEntryRequestedAsync(CaptainLoggerEvArgs<object> evArgs)
    {
        string? stackTrace = null;

        if (evArgs.Exception is not null)
        {
            stackTrace = $"{evArgs.Exception}";
        }

        var logObject = evArgs.State;

        var list = evArgs.State as IReadOnlyList<KeyValuePair<string, object>>;

        if (list is not null &&
            list.FirstOrDefault(x => x.Value is LogEntry)
                .Value is LogEntry logEntry)
        {
            logObject = logEntry.Tags;
        }

        if (list is not null &&
            list.Where(x => x.Key.StartsWith("Arg")).Count() == 2)
        {
            logObject = new
            {
                Message = list.First(x => x.Key == "Arg0").Value,
                JsonPayLoad = list.First(x => x.Key == "Arg1").Value
            };
        }

        var ddEntry = new DataDogEntry(
            SERVICE,
            _hostName,
            $"{evArgs.LogLevel}",
            evArgs.LogCategory,
            $"timeStamp:{evArgs.LogTime:yyyy-MM-ddTHH:mm:ss.fff},eventId{evArgs.LogEvent}",
            logObject,
            stackTrace
            );

        _ = await _clients
            .DataDogLogs
            .PostAsync<object>(
                "logs",
                ddEntry);

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