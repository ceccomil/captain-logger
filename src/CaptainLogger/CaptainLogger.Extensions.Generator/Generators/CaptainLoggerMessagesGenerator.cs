namespace CaptainLogger.Extensions.Generator.Generators;

[Generator]
internal sealed class CaptainLoggerMessagesGenerator : IIncrementalGenerator
{
  private static void Execute(
    SourceProductionContext context,
    ImmutableArray<LoggerOptionsDefinition> lods,
    bool nullable)
  {
    if (lods.IsDefaultOrEmpty)
    {
      return;
    }


  }

  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    context.RegisterSourceOutput(
      context.GetBaseProvider(),
      (x, y) => Execute(x, y.Options, y.IsNullable));
  }
}
