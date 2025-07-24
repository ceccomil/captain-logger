global using CaptainLogger.Contracts.EventsArgs;
global using CaptainLogger.Contracts.Options;
global using CaptainLogger.Helpers;
global using CaptainLogger.LoggingLogic;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Configuration;
global using Microsoft.Extensions.Options;
global using System.Buffers;
global using System.Collections;
global using System.Collections.Concurrent;
global using System.Text;
global using System.Text.Json;
global using static CaptainLogger.Helpers.InternalGlobals;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CaptainLogger.Tests")]
