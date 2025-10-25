using CaptainLogger.Contracts.Options;

namespace WebApiUseCase;

public static class Bootstrapping
{
  public static IHostApplicationBuilder AddRegistrations(
    this IHostApplicationBuilder builder)
  {
    builder
      .Logging
      .ClearProviders()
      .AddCaptainLogger()
      .AddFilter("Microsoft", LogLevel.Warning)
      .AddFilter("System", LogLevel.Error)
      .AddFilter("WebApiUseCase", LogLevel.Debug);

    builder
      .Services
      .AddSingleton<ScopedStateMiddleware>()
      .AddScoped<IInjectedEndpoints, InjectedEndpoints>()
      .Configure<CaptainLoggerOptions>(x =>
      {
        //x.HighPerfStructuredLogging = true;
        //x.StructuredLogMetadata.Add("Application", "WebApiUseCase");
        x.IncludeScopes = true;
      });

    return builder;
  }

  public static IApplicationBuilder UseRegistrations(
    this IApplicationBuilder app)
  {
    app
      .UseMiddleware<ScopedStateMiddleware>()
      .UseHttpsRedirection()
      .MapEndpoints();

    return app;
  }

  private static IApplicationBuilder MapEndpoints(
    this IApplicationBuilder app)
  {
    if (app is not WebApplication webApp)
    {
      throw new InvalidOperationException(
        "The application builder is not a web application.");
    }

    webApp.MapGet("/weatherforecast", (
      [FromServices] IInjectedEndpoints ep,
      CancellationToken ct) => ep.GetWeatherForecast(ct));

    return app;
  }
}
