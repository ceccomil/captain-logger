CaptainLogger - a simple but effective logger for .Net
======================================================

------------------------------------------------------------------
Source: [GitHub repo](https://github.com/ceccomil/captain-logger/)

Packages
--------
| Package | NuGet Stable | NuGet Pre-release | Downloads |
| ------- | ------------ | ----------------- | --------- | 
| [CaptainLogger](https://www.nuget.org/packages/CaptainLogger/) | [![CaptainLogger](https://img.shields.io/nuget/v/CaptainLogger.svg)](https://www.nuget.org/packages/CaptainLogger/) | [![CaptainLogger](https://img.shields.io/nuget/vpre/CaptainLogger.svg)](https://www.nuget.org/packages/CaptainLogger/) | [![CaptainLogger](https://img.shields.io/nuget/dt/CaptainLogger.svg)](https://www.nuget.org/packages/CaptainLogger/) |
| [CaptainLogger.Contracts](https://www.nuget.org/packages/CaptainLogger.Contracts/) | [![CaptainLogger.Contracts](https://img.shields.io/nuget/v/CaptainLogger.Contracts.svg)](https://www.nuget.org/packages/CaptainLogger.Contracts/) | [![CaptainLogger.Contracts](https://img.shields.io/nuget/vpre/CaptainLogger.Contracts.svg)](https://www.nuget.org/packages/CaptainLogger.Contracts/) | [![CaptainLogger.Contracts](https://img.shields.io/nuget/dt/CaptainLogger.Contracts.svg)](https://www.nuget.org/packages/CaptainLogger.Contracts/) |
| [CaptainLogger.Extensions.Generator](https://www.nuget.org/packages/CaptainLogger.Extensions.Generator/) | [![CaptainLogger.Extensions.Generator](https://img.shields.io/nuget/v/CaptainLogger.Extensions.Generator.svg)](https://www.nuget.org/packages/CaptainLogger.Extensions.Generator/) | [![CaptainLogger.Extensions.Generator](https://img.shields.io/nuget/vpre/CaptainLogger.Extensions.Generator.svg)](https://www.nuget.org/packages/CaptainLogger.Extensions.Generator/) | [![CaptainLogger.Extensions.Generator](https://img.shields.io/nuget/dt/CaptainLogger.Extensions.Generator.svg)](https://www.nuget.org/packages/CaptainLogger.Extensions.Generator/) |
| [CaptainLogger.RequestTracer](https://www.nuget.org/packages/CaptainLogger.RequestTracer/) | [![CaptainLogger.RequestTracer](https://img.shields.io/nuget/v/CaptainLogger.RequestTracer.svg)](https://www.nuget.org/packages/CaptainLogger.RequestTracer/) | [![CaptainLogger.RequestTracer](https://img.shields.io/nuget/vpre/CaptainLogger.RequestTracer.svg)](https://www.nuget.org/packages/CaptainLogger.RequestTracer/) | [![CaptainLogger.RequestTracer](https://img.shields.io/nuget/dt/CaptainLogger.RequestTracer.svg)](https://www.nuget.org/packages/CaptainLogger.RequestTracer/) |

Features
--------
- Colorful console logs
- Minimal dependencies (requires [.NET8](https://github.com/dotnet/core/blob/main/release-notes/8.0/8.0.0/8.0.0.md))
- Older versions < 3.0 are [.NET7](https://github.com/dotnet/core/blob/main/release-notes/7.0/7.0.0/7.0.0.md) dependant
- Older versions < 2.0 are [.NET6](https://github.com/dotnet/core/blob/main/release-notes/6.0/6.0.1/6.0.1.md?WT.mc_id=dotnet-35129-website) dependant
- Handy (generics) extensions auto-generated based on configuration
- Sync and Async EventHandlers to easily handle log entries

Optional Features
-------------------
- Simple middleware to handle trace identifiers correlation between HTTP requests (see example: CaptainLogger.CentralizedLogging.Api)

Minimum configuration
=====================================
-------------------------------------

```csharp
...
var builder = WebApplication.CreateBuilder(args);

builder
    .Logging
    .ClearProviders()
    .AddCaptainLogger()
    .AddFilter("System", LogLevel.Error)
    .AddFilter("Microsoft", LogLevel.Warning)
    .AddFilter("", LogLevel.Information); //No namespace with top level statements
var app = builder.Build();
...
```
See example: CaptainLogger.MinimalApi

Static templates configuration
=====================================
-------------------------------------

```csharp
...
.Configure<CaptainLoggerOptions>(opts =>
{
    opts.TimeIsUtc = true;
    opts.ArgumentsCount = LogArguments.Four;
    opts.Templates.Add(LogArguments.Three, "Request subbmitted by user id {UserId}, of department [{DepId}] - client id {ClientId}");
    opts.Templates.Add(LogArguments.Four, "Method [{Method}] - URL: {Hostname}:{Port} - status code returned: {StatusCode}");
    opts.LogRecipients = Recipients.Console | Recipients.File;
})
.AddLogging(builder =>
{
    builder
        .ClearProviders()
        .AddCaptainLogger()
        .AddFilter(typeof(Program).Namespace, LogLevel.Information);
})
...
```
See example: CaptainLogger.Templates

EventHandlers configuration
=====================================
-------------------------------------

```csharp
...
.Configure<CaptainLoggerOptions>(opts =>
{
   // Raise async events on log entries (Default is false)
   opts.TriggerAsyncEvents = true;
   
   // Raise sync events on log entries (Default is false)
   opts.TriggerEvents = true;
})
...
```

Example of use:
```csharp
    private async Task SomeLoggingWithEventHandlers()
    {
        void LogEntryRequested(CaptainLoggerEvArgs<object> evArgs)
        {
            //Some long operations
            Thread.Sleep(500);
            Console.WriteLine($"SYNC HANDLER: {evArgs.LogTime:ss.fff} - {evArgs.State}");
        }

        async Task LogEntryRequestedAsync(CaptainLoggerEvArgs<object> evArgs)
        {
            //Some long operations
            await Task.Delay(1000);
            Console.WriteLine($"ASYNC HANDLER: {evArgs.LogTime:ss.fff} - {evArgs.State}");
        }

        //ASYNC HANDLERS
        _logger.LogEntryRequestedAsync += LogEntryRequestedAsync;

        await Task.Delay(100);
        _logger.InformationLog("Simple message no arguments!");

        await Task.Delay(100);
        _logger.ErrorLog("Simple error message", new NotImplementedException("Test Exception"));

        await Task.Delay(3000);
        _logger.LogEntryRequestedAsync -= LogEntryRequestedAsync;

        Console.WriteLine();

        //SYNC HANDLERS
        _logger.LogEntryRequested += LogEntryRequested;

        await Task.Delay(100);
        _logger.InformationLog("Mex for Sync Handler");

        await Task.Delay(100);
        _logger.ErrorLog("Exception for Sync Handler", new NotImplementedException("Test Exception"));

        _logger.LogEntryRequested -= LogEntryRequested;
    }
```
See examples:
- CaptainLogger.CentralizedLogging.Api (Save logs to [DataDog](https://www.datadoghq.com))
- CaptainLogger.SaveLogs (Save logs to [LiteDB](https://github.com/mbdavid/LiteDB))

TraceIdentifier correlation
=====================================
-------------------------------------

```csharp
...
   services.AddCaptainLoggerRequestTracer();
...
   app.UseCaptainLoggerRequestTracer();
...
```

Example of use:
```csharp
    // TraceIdentifier will be handled by the middleware if sent with the specific header in the request.

    _logger
        .InformationLog(
        $"New request received with trace identifier: {HttpContext.TraceIdentifier}");
```

To send an already existing TraceIdentifier to a different service when sending requests:
```csharp
    // _correlationHeader is an injected `ICorrelationHandler`
    // _clientFactory is an injected `IHttpClientFactory`
    var client = _clientFactory.CreateClient("WeatherClient");
    _correlationHeader.Append(client);
```
See examples:
- CaptainLogger.CentralizedLogging.Api (Handles incoming trace identifier, and when sending requests))
