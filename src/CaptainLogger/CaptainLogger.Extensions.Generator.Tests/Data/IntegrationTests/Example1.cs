using Microsoft.Extensions.DependencyInjection;
using CaptainLogger.Contracts.Options;

namespace CaptainLogger.Extensions.Generator.Tests.Data.IntegrationTests;

internal static class Example1
{
  // Very odd formatting
  public static void Run()
  {
    var sc = new ServiceCollection()
      .Configure<CaptainLoggerOptions>(
        x => 
          x.ArgumentsCount = LogArguments.One);
  }
}
