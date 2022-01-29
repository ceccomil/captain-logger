using CaptainLogger;
using CaptainLogger.Contracts;
using CaptainLogger.Options;
using CaptainLogger.SaveLogs.Logging;

var builder = WebApplication.CreateBuilder(args);

builder
    .Logging
    .ClearProviders()
    .AddCaptainLogger()
    .AddFilter("System", LogLevel.Error)
    .AddFilter("Microsoft", LogLevel.Warning)
    .AddFilter("", LogLevel.Information); //No namespace with top level statements


builder.Services.AddControllers();

builder
    .Services
    .Configure<CaptainLoggerOptions>(x => x.LogRecipients = Recipients.Console)
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddSingleton<ICaptainLoggerHandler, LogEntryHandler>()
    .AddSingleton<IRepo, Repo>();
    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
