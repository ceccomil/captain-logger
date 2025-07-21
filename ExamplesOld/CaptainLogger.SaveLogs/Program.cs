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
    .Configure<CaptainLoggerOptions>(options =>
    {
        options.LogRecipients = Recipients.Console;
        options.ArgumentsCount = LogArguments.One;
        options.Templates.Add(LogArguments.One, "Static counter has been increased! New value: {Value}");
        options.TriggerAsyncEvents = true;
    })
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddScoped<IRepo, Repo>()
    .AddScoped(typeof(ILogHandler<>), typeof(LogHandler<>));
    
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
