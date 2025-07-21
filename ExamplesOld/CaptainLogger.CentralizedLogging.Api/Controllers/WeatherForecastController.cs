using CaptainLogger.CentralizedLogging.Api.Contracts;
using CaptainLogger.RequestTracer.Headers;
using EazyHttp;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CaptainLogger.CentralizedLogging.Api.Controllers;

[ApiController]
[Route("[controller]")]
[TypeFilter(typeof(ApiExceptionFilter))]
public class WeatherForecastController : ControllerBase
{
    private readonly static string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly",
        "Cool", "Mild", "Warm", "Balmy",
        "Hot", "Sweltering", "Scorching"
    };

    private readonly ICaptainLogger _logger;
    private readonly ICorrelationHandler _correlationHeader;
    private readonly IEazyClients _clients;

    public WeatherForecastController(
        ICaptainLogger<WeatherForecastController> logger,
        ICorrelationHandler correlationHeader,
        IEazyClients httpClients)
    {
        _logger = logger;
        _correlationHeader = correlationHeader;
        _clients = httpClients;
    }

    [HttpGet("{days:int}")]
    public async Task<IEnumerable<WeatherForecast>> Get([FromRoute] int days)
    {
        var logEntry = new LogEntry()
        {
            Message = "Request received Weateher forecast on its way",
            TraceId = HttpContext.TraceIdentifier,
            CorrelationId = Guid.NewGuid(),
            SourceMethod = $"{nameof(WeatherForecastController)}.Get",
            Env = "Development",
            Host = Request.Host.Value
        };

        _logger.DebugLog(logEntry);

        if (days <= 0)
        {
            throw new NotSupportedException("The minimum accepted number of days is 1!");
        }

        if (days > 10)
        {
            throw new NotSupportedException("The maximum accepted number of days is 10!");
        }

        if (days == 5)
        {
            _logger
                .WarningLog(
                    "Request is triggering another request!");

            return await SubsequentCall();
        }

        var forecasts = Enumerable.Range(1, days).Select(index => new WeatherForecast()
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        _logger
            .InformationLog(
                $"Forecast requested {days} day(s)",
                JsonSerializer.Serialize(forecasts));

        return forecasts;
    }

    private async Task<IEnumerable<WeatherForecast>> SubsequentCall()
    {
        var requestUrl = $"{Request.Scheme}://{Request.Host.Value}/WeatherForecast/6";

        _correlationHeader
            .Append(
                _clients
                .Http
                .HttpClient);

        return await _clients
            .Http
            .GetAsync<IEnumerable<WeatherForecast>>(requestUrl)
            ?? throw new NullReferenceException();
    }
}
