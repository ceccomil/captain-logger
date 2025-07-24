using Microsoft.Extensions.DependencyInjection;
using CaptainLogger.Contracts.Options;

namespace CaptainLogger.Extensions.Generator.Tests.Data.IntegrationTests;

internal class Example
{
  public static void Main()
  {
    var sc = new ServiceCollection()
      .Configure<CaptainLoggerOptions>(x => x.TimeIsUtc = true);
  }
}
