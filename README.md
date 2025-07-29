CaptainLogger - .Net logging library
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

---

## Table of Contents
- [Introduction](#introduction)
- [Installation](#installation)
- [Minimum Configuration](#minimum-configuration)
- [Static Templates Configuration](#static-templates-configuration)
- [EventHandlers Configuration](#eventhandlers-configuration)
- [Example Usage](#example-usage)

## Introduction
CaptainLogger is a powerful and flexible .NET logging library designed to simplify logging in your applications. It supports advanced features like static templates, event handlers, and customizable logging levels, making it suitable for a wide range of use cases.

## Installation
To install CaptainLogger, use the following NuGet command:

```bash
Install-Package CaptainLogger
```

For additional packages:

- `CaptainLogger.Extensions.Generator`: `Install-Package CaptainLogger.Extensions.Generator`

## Minimum Configuration
To get started with CaptainLogger, use the following configuration as an example:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder
    .Logging
    .ClearProviders()
    .AddCaptainLogger()
    .AddFilter("System", LogLevel.Error)
    .AddFilter("Microsoft", LogLevel.Warning)
    .AddFilter("", LogLevel.Information); // No namespace with top-level statements

var app = builder.Build();
```

## Static Templates Configuration (Requires Generator Package)

```csharp
.Configure<CaptainLoggerOptions>(opts =>
{
    opts.TimeIsUtc = true;
    opts.ArgumentsCount = LogArguments.Four;
    opts.Templates.Add(LogArguments.Three, "Request submitted by user id {UserId}, of department [{DepId}] - client id {ClientId}");
    opts.Templates.Add(LogArguments.Four, "Method [{Method}] - URL: {Hostname}:{Port} - status code returned: {StatusCode}");
    opts.LogRecipients = Recipients.Console | Recipients.File;
})
.AddLogging(builder =>
{
    builder
        .ClearProviders()
        .AddCaptainLogger()
        .AddFilter(typeof(Program).Namespace, LogLevel.Information);
});
```

## EventHandlers Configuration

```csharp
.Configure<CaptainLoggerOptions>(opts =>
{
   // Raise async events on log entries (Default is false)
   opts.TriggerAsyncEvents = true;
});
```

## Example Usage

```csharp
public interface ILoggerReceiver
{
}

internal sealed class LoggerReceiver : ILoggerReceiver
{
  public LoggerReceiver(ILogDispatcher dispatcher)
  {
    dispatcher.OnLogEntry += OnLogEntry;
  }

  private Task OnLogEntry(CaptainLoggerEventArgs<object> evArgs)
  {
    var myLogEntry = new
    {
      evArgs.Exception,
      evArgs.CorrelationId,
      evArgs.LogCategory,
      evArgs.LogLevel,
      evArgs.State,
      evArgs.LogTime
    };

    // Here you can process the log entry as needed.
    return Task.CompletedTask;
  }
}
```
