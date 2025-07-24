namespace CaptainLogger.Extensions.Generator.Helpers;

internal static class Receiver
{
  public static bool IsCaptainLoggerOptionsConfiguration(
    this SyntaxNode node)
  {
    if (node is not GenericNameSyntax nameSyntax ||
      nameSyntax.Identifier.Text != SERVICES_CONFIGURE)
    {
      return false;
    }

    var typeArg = nameSyntax
      .TypeArgumentList
      .Arguments
      .OfType<IdentifierNameSyntax>()
      .FirstOrDefault();

    return typeArg?.Identifier.Text == nameof(CaptainLoggerOptions);
  }

  public static LoggerOptionsDefinition GetOptionsDefinition(
    this GeneratorSyntaxContext context)
  {
    var exprSyntax = ((GenericNameSyntax)context.Node)
      .Ancestors()
      .OfType<InvocationExpressionSyntax>()
      .FirstOrDefault();

    var lambda = exprSyntax?
      .ArgumentList
      .Arguments
      .FirstOrDefault()?
      .ChildNodes()
      .OfType<SimpleLambdaExpressionSyntax>()
      .FirstOrDefault();

    if (lambda is null)
    {
      return new(0, []);
    }

    var argumentsCount = lambda
      .Body
      .DescendantNodes()
      .OfType<MemberAccessExpressionSyntax>()
      .FirstOrDefault(x =>
        x.IsKind(SyntaxKind.SimpleMemberAccessExpression) &&
        x.Name.Identifier.Text == nameof(CaptainLoggerOptions.ArgumentsCount))?
      .Parent?
      .DescendantNodes()
      .OfType<MemberAccessExpressionSyntax>()
      .FirstOrDefault(x => x.Expression.ToString() == nameof(LogArguments))?
      .Name
      .ToString()
      .ToArgumentsCount()
      ?? 0;

    var templates = lambda
      .Body
      .DescendantNodes()
      .OfType<MemberAccessExpressionSyntax>()
      .Where(x =>
        x.IsKind(SyntaxKind.SimpleMemberAccessExpression) &&
        x.Name.Identifier.Text == nameof(CaptainLoggerOptions.Templates))
      .Select(x => x.Parent?.Parent as InvocationExpressionSyntax)
      .Where(x => x is not null && x.ArgumentList.Arguments.Count == 2)
      .ToDictionary(
        x => x!.ArgumentList.Arguments[0].ToArgumentsCount(),
        x => x!.ArgumentList.Arguments[1].Expression)
      ?? [];

    return new(argumentsCount, templates, lambda.GetLocation());
  }

  private static int ToArgumentsCount(this ArgumentSyntax? argument)
  {
    return argument?
      .DescendantNodes()
      .OfType<MemberAccessExpressionSyntax>()
      .FirstOrDefault(x => x.Expression.ToString() == nameof(LogArguments))
      .Name
      .ToString()
      .ToArgumentsCount() ?? 0;
  }

  private static int ToArgumentsCount(this string? value)
  {
    return value switch
    {
      nameof(LogArguments.One) => 1,
      nameof(LogArguments.Two) => 2,
      nameof(LogArguments.Three) => 3,
      nameof(LogArguments.Four) => 4,
      nameof(LogArguments.Five) => 5,
      nameof(LogArguments.Six) => 6,
      _ => 0
    };
  }
}