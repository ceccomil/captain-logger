using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CaptainLogger.CentralizedLogging.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(ApiExceptionFilter))]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly",
            "Cool", "Mild", "Warm", "Balmy",
            "Hot", "Sweltering", "Scorching"
        };

        private readonly ICaptainLogger _logger;

        public WeatherForecastController(ICaptainLogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{days:int}")]
        public IEnumerable<WeatherForecast> Get([FromRoute] int days)
        {
            if (days <= 0)
                throw new NotSupportedException("The minimum accepted number of days is 1!");

            if (days > 10)
                throw new NotSupportedException("The maximum accepted number of days is 10!");

            var forecasts = Enumerable.Range(1, days).Select(index => new WeatherForecast()
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            _logger
                .InformationLog($"Forecast requested {days} day(s){Environment.NewLine}" +
                JsonSerializer.Serialize(forecasts));

            return forecasts;
        }
    }
}