using CaptainLogger;
using CaptainLogger.Contracts.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VerySimpleUseCase;

var builder = new HostApplicationBuilder();

builder
  .Logging
  .ClearProviders()
  .AddCaptainLogger()
  .AddFilter("", LogLevel.Trace);

builder
  .Services
  .AddHttpClient()
  .Configure<CaptainLoggerOptions>(x =>
  {
    x.HighPerfStructuredLogging = true;
    x.TimeIsUtc = true;
    x.StructuredLogMetadata.Add("service-name", "sample-logging-case");
    x.StructuredLogMetadata.Add("deployment", new { subscription = "0038B8FE-BCBB-444D-B0B8-31E6B6122039", tenant = "DA0D2D7C-6457-4EF8-93EE-22CD108308C0", env = "dev" });
  })
  .AddHostedService<LoggerTest>();

var app = builder.Build();

await app.RunAsync();