namespace CaptainLogger.Extensions.Generator.Definitions;

internal sealed class LoggerOptionsDefinition(
  int argumentsCount,
  Dictionary<int, ExpressionSyntax> templates)
{
  public int ArgumentsCount { get; } = argumentsCount;
  public Dictionary<int, ExpressionSyntax> Templates { get; } = templates;
}
