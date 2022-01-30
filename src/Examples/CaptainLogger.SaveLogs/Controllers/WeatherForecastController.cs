using CaptainLogger.Contracts.EventArguments;
using CaptainLogger.SaveLogs.Logging;
using Microsoft.AspNetCore.Mvc;

namespace CaptainLogger.SaveLogs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly",
            "Cool", "Mild", "Warm", "Balmy",
            "Hot", "Sweltering", "Scorching"
        };

        private readonly ICaptainLogger _logger;

        public WeatherForecastController(
            ICaptainLogger<WeatherForecastController> logger,
            ILogHandler logHandler)
        {
            _logger = logger;
            logHandler.SubscribeToLoggerEvents();
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.InformationLog("A new request to the GetWeatherForecast action has been made!");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}