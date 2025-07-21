global using CaptainLogger.RequestTracer.Headers;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Primitives;
global using static CaptainLogger.RequestTracer.Globals.Constants;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CaptainLogger.RequestTracer.Tests")]
