global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Primitives;
global using Microsoft.Extensions.DependencyInjection;
global using static CaptainLogger.RequestTracer.Globals.Constants;
global using Microsoft.AspNetCore.Builder;
global using CaptainLogger.RequestTracer.Headers;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CaptainLogger.RequestTracer.Tests")]
