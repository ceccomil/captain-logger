using CaptainLogger;
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
  .AddHostedService<LoggerTest>();

var app = builder.Build();

await app.RunAsync();