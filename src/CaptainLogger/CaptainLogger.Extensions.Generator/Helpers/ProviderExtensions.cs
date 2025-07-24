namespace CaptainLogger.Extensions.Generator.Helpers;

internal static class ProviderExtensions
{
  public static IncrementalValueProvider<BaseProvider> GetBaseProvider(
    this IncrementalGeneratorInitializationContext context)
  {
    var definitionsProvider = context
      .SyntaxProvider
      .CreateSyntaxProvider(
        predicate: static (x, _) => x.IsCaptainLoggerOptionsConfiguration(),
        transform: static (x, _) => x.GetOptionsDefinition())
      .Where(x => x is not null)
      .Collect();

    var nullableProvider = context
      .CompilationProvider
      .Select(static (x, _) =>
        x.Options.NullableContextOptions is not NullableContextOptions.Disable);

    var combinedProvider = definitionsProvider
      .Combine(nullableProvider);

    return combinedProvider;
  }
}