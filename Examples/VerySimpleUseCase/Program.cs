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
  .AddCaptainLogger();

builder
  .Services
  .Configure<CaptainLoggerOptions>(x =>
  {
    x.HighPerfStructuredLogging = true;
    //x.IncludeFormattedMessageInHighPerfLogging = true;
    x.TimeIsUtc = true;
  })
  .AddHostedService<LoggerTest>();

var app = builder.Build();

await app.RunAsync();