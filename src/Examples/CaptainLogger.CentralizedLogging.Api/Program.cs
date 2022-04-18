using CaptainLogger;
using CaptainLogger.CentralizedLogging.Api.Services;
using CaptainLogger.Options;
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
    .AddCaptainLoggerRequestTracer()
    .AddControllers();

builder
    .Services
    .Configure<CaptainLoggerOptions>(opts =>
    {
        opts.TimeIsUtc = true;
        opts.TriggerAsyncEvents = true;
        opts.LogRecipients = Recipients.Console;
    })
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddSingleton<IDataDogLogger, DataDogLogger>()
    .AddHostedService(sp => sp.GetRequiredService<IDataDogLogger>())
    .AddHttpClient();
    

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app
        .UseSwagger()
        .UseSwaggerUI();
}

app
    .UseCaptainLoggerRequestTracer()
    .UseHttpsRedirection()
    .UseAuthorization();

app
    .MapControllers();

app
    .Run();
