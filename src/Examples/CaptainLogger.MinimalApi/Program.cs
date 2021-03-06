using CaptainLogger;
using CaptainLogger.RequestTracer;

var builder = WebApplication.CreateBuilder(args);

builder
    .Logging
    .ClearProviders()
    .AddCaptainLogger()
    .AddFilter("System", LogLevel.Error)
    .AddFilter("Microsoft", LogLevel.Warning)
    .AddFilter("", LogLevel.Information); //No namespace with top level statements

builder
    .Services
    .AddCaptainLoggerRequestTracer("X-MyTraceID");

var app = builder.Build();

app
    .UseCaptainLoggerRequestTracer()
    .UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly",
    "Cool", "Mild", "Warm", "Balmy",
    "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (ILogger<WeatherForecast> logger, IHttpContextAccessor ctx) =>
{
    var temp = Random.Shared.Next(-20, 55);
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           temp,
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();

    logger.LogInformation("Request trace identifier {}", ctx.HttpContext?.TraceIdentifier);

    logger.LogInformation("Captain logger injected as a `{MsILogger}`", typeof(ILogger).FullName);
    logger.LogInformation("[GET] temperature returned: {Temp}", temp);

    return forecast;
});

app.MapGet("/weatherforecast/temps", (ICaptainLogger<TempForecast> logger, IHttpContextAccessor ctx) =>
{
    var temp = Random.Shared.Next(-20, 55);
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new TempForecast
       (
           DateTime.Now.AddDays(index),
           temp
       ))
        .ToArray();

    logger.InformationLog($"Request trace identifier: {ctx.HttpContext?.TraceIdentifier}");

    logger.InformationLog($"Captain logger injected as a `{typeof(ICaptainLogger).FullName}`");
    logger.InformationLog($"[GET] temperature returned: {temp}");

    return forecast;
});

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary) : TempForecast(Date, TemperatureC);

internal record TempForecast(DateTime Date, int TemperatureC)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}