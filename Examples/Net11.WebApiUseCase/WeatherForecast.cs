namespace WebApiUseCase;

public sealed record WeatherForecast(DateOnly Date)
{
  private static readonly ImmutableArray<string> _summaries =
  [
    "Freezing", "Bracing", "Chilly", "Cool",
    "Mild", "Warm", "Balmy", "Hot", "Sweltering",
    "Scorching"
  ];

  public int TemperatureC { get; } = Random.Shared.Next(-20, 55);
  public string Summary { get; } = _summaries[Random.Shared.Next(_summaries.Length)];
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
