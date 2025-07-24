using Microsoft.Extensions.DependencyInjection;
using CaptainLogger.Contracts.Options;
using System.Security.Cryptography.X509Certificates;

namespace CaptainLogger.Extensions.Generator.Tests.Data.IntegrationTests;

internal static class Example3
{
  public const string ExampleName = "Example: {argument1} - {value2}";

  // static fqn const for template
  public static void Run()
  {
    var sc = new ServiceCollection().Configure<CaptainLoggerOptions>(x =>
    {
      x.ArgumentsCount = LogArguments.Two;
      
      x.Templates.Add(
        LogArguments.Two,
        CaptainLogger.Extensions.Generator.Tests.Data.IntegrationTests.Example3.ExampleName);
    });
  }
}
