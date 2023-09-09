using CaptainLogger;
using CaptainLogger.CentralizedLogging.Api.Services;
using CaptainLogger.Options;
using CaptainLogger.RequestTracer;
using EazyHttp;
using EazyHttp.Contracts;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder
    .Logging
    .ClearProviders()
    .AddCaptainLogger()
    .AddFilter("System", LogLevel.Error)
    .AddFilter("Microsoft", LogLevel.Warning)
    .AddFilter("", LogLevel.Debug); //No namespace with top level statements

// To test this a valid DataDog account is required
// https://docs.datadoghq.com/api/latest/logs/#send-logs
// DD_ENDPOINT and API KEY must be valid (accordingly to the instruction above)
// !! make sure the URL is for the region you want!

builder
    .Services
    .ConfigureEazyHttpClients(opts =>
    {
        opts
            .EazyHttpClients.Add(
                new(
                    "DataDogLogs",
                    "https://http-intake.logs.datadoghq.eu/api/v2"));

        opts
            .PersistentHeaders
            .Add(
                "DataDogLogs",
                new RequestHeader[]
                {
                    new(
                        HeaderNames.Accept,
                        "application/json"),
                    new(
                        "DD-API-KEY",
                        "<ApiKey>")
                });

        opts
            .SerializersOptions
            .Add(
                "DataDogLogs",
                new(
                    JsonSerializerDefaults.Web)
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

        opts
            .Retries
            .Add(
                "DataDogLogs",
                new()
                {
                    MaxAttempts = 4,
                    StatusCodeMatchingCondition = (code, method) =>
                    {
                        if (method != HttpMethod.Post)
                        {
                            return false;
                        }

                        if (code is HttpStatusCode.ServiceUnavailable
                            or HttpStatusCode.GatewayTimeout
                            or HttpStatusCode.RequestTimeout)
                        {
                            return true;
                        }

                        return false;
                    }
                });

        opts
            .EazyHttpClients
                .Add(new("Http"));
    })
    .AddEazyHttpClients()
    .AddCaptainLoggerRequestTracer()
    .AddControllers();

builder
    .Services
    .Configure<CaptainLoggerOptions>(opts =>
    {
        opts.TimeIsUtc = true;
        opts.TriggerAsyncEvents = true;
        opts.LogRecipients = Recipients.Console;
        opts.ArgumentsCount = LogArguments.Two;
        opts.Templates.Add(LogArguments.One, "{Arg0}");
        opts.Templates.Add(LogArguments.Two, "{Arg0}" + Environment.NewLine + "{Arg1}");
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
