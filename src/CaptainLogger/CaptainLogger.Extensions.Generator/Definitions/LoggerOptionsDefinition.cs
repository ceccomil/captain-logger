namespace CaptainLogger.Extensions.Generator.Definitions;

internal sealed class LoggerOptionsDefinition(
  int argumentsCount,
  Dictionary<int, ExpressionSyntax> templates,
  Location? location = null)
{
  public int ArgumentsCount { get; } = argumentsCount;
  public Dictionary<int, ExpressionSyntax> Templates { get; } = templates;
  public Location? ConfigureLocation { get; } = location;
}
