using Microsoft.Extensions.DependencyInjection;
using CaptainLogger.Contracts.Options;
using System.Security.Cryptography.X509Certificates;

namespace CaptainLogger.Extensions.Generator.Tests.Data.IntegrationTests;

internal static class Example2
{
  // One argument but template set for two arguments
  public static void Run()
  {
    var sc = new ServiceCollection().Configure<CaptainLoggerOptions>(x =>
    {
      x.ArgumentsCount = LogArguments.One;
      x.Templates.Add(LogArguments.Two, "{arg1} - {arg2}");
    });
  }
}
