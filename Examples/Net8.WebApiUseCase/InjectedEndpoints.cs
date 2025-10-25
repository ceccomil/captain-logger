namespace WebApiUseCase;

public interface IInjectedEndpoints
{
  Task<IEnumerable<WeatherForecast>> GetWeatherForecast(
    CancellationToken cancellationToken);
}

internal class InjectedEndpoints(
  ILogger<InjectedEndpoints> logger) : IInjectedEndpoints
{
  private readonly ILogger _logger = logger;

  public Task<IEnumerable<WeatherForecast>> GetWeatherForecast(
    CancellationToken cancellationToken)
  {
    if (cancellationToken.IsCancellationRequested)
    {
      return Task
        .FromCanceled<IEnumerable<WeatherForecast>>(cancellationToken);
    }

    var forecast = Enumerable
        .Range(1, 5)
        .Select(x => new WeatherForecast(GetDate(x)));

    foreach (var item in forecast)
    {
      _logger.LogInformation(
        "Generated weather forecast for {Date}",
        item.Date);
    }

    _logger.LogInformation(
      "Returning weather forecasts");

    return Task.FromResult(forecast);
  }

  private static DateOnly GetDate(int daysFromNow)
  {
    var date = DateTime
      .Now
      .AddDays(daysFromNow);

    return DateOnly.FromDateTime(date);
  }
}
