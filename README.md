CaptainLogger - a simple but effective logger for .Net
======================================================

------------------------------------------------------------------
Source: [GitHub repo](https://github.com/ceccomil/captain-logger/)

Packages
--------
| Package | NuGet Stable | NuGet Pre-release | Downloads |
| ------- | ------------ | ----------------- | --------- | 
| [CaptainLogger](https://www.nuget.org/packages/CaptainLogger/) | -| [![CaptainLogger](https://img.shields.io/nuget/vpre/CaptainLogger.svg)](https://www.nuget.org/packages/CaptainLogger/) | [![CaptainLogger](https://img.shields.io/nuget/dt/CaptainLogger.svg)](https://www.nuget.org/packages/CaptainLogger/) |
| [CaptainLogger.Contracts](https://www.nuget.org/packages/CaptainLogger.Contracts/) | -| [![CaptainLogger.Contracts](https://img.shields.io/nuget/vpre/CaptainLogger.Contracts.svg)](https://www.nuget.org/packages/CaptainLogger.Contracts/) | [![CaptainLogger.Contracts](https://img.shields.io/nuget/dt/CaptainLogger.Contracts.svg)](https://www.nuget.org/packages/CaptainLogger.Contracts/) |
| [CaptainLogger.Extensions.Generator](https://www.nuget.org/packages/CaptainLogger.Extensions.Generator/) | -| [![CaptainLogger.Extensions.Generator](https://img.shields.io/nuget/vpre/CaptainLogger.Extensions.Generator.svg)](https://www.nuget.org/packages/CaptainLogger.Extensions.Generator/) | [![CaptainLogger.Extensions.Generator](https://img.shields.io/nuget/dt/CaptainLogger.Extensions.Generator.svg)](https://www.nuget.org/packages/CaptainLogger.Extensions.Generator/) |

Features
--------
- Colorful console logs
- Minimal dependencies (requires [.NET6](https://github.com/dotnet/core/blob/main/release-notes/6.0/6.0.1/6.0.1.md?WT.mc_id=dotnet-35129-website))
- Handy (generics) extensions auto-generated based on configuration

Minimum configuration:

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