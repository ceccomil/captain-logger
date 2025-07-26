namespace CaptainLogger.Helpers;

internal static class ConsoleColourPolicy
{
  public static readonly bool UseAnsi =
    !Console.IsOutputRedirected &&
    !IsColourDisabledByEnv();

  private static bool IsColourDisabledByEnv()
  {
    if (Environment.GetEnvironmentVariable("NO_COLOR") is { Length: > 0 })
    {
      return true;
    }

    var term = Environment.GetEnvironmentVariable("TERM")
      ?? "";

    if (term.Equals("dumb", StringComparison.OrdinalIgnoreCase))
    {
      return true;
    }

    return false;
  }
}
