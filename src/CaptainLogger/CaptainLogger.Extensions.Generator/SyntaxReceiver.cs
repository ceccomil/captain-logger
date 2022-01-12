namespace CaptainLogger.Extensions.Generator;

public class SyntaxReceiver : ISyntaxContextReceiver
{
    public List<string> GeneratorLogger { get; } = new();
    public int ArgumentsCount { get; private set; } = 0;

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is InvocationExpressionSyntax ies &&
            $"{ies}" is string code &&
            (code.Contains("Configure<CaptainLoggerOptions>") ||
            code.Contains("Configure<CaptainLogger.Options.CaptainLoggerOptions>")))
        {
            GeneratorLogger.Add($"Found: {code}{Environment.NewLine}");

            var lambda = ies
                .DescendantNodes()
                .OfType<LambdaExpressionSyntax>()
                .FirstOrDefault(x => $"{x}".Contains(".ArgumentsCount"));

            if (lambda is null)
                return;
                
            GeneratorLogger
                .Add($"Lambda: {lambda}{Environment.NewLine}");

            ArgumentsCount = GetArgumentsCount(lambda);
            GeneratorLogger.Add($"ArgumentsCount: {ArgumentsCount}");
        }
    }

    private int GetArgumentsCount(LambdaExpressionSyntax lambda)
    {
        var rgEx = new Regex(@"^.*ArgumentsCount.*=.*LogArguments.*\.([a-z]+)$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        var rows = lambda
            .DescendantNodes()
            .OfType<AssignmentExpressionSyntax>();

        foreach (var r in rows)
        {
            var m = rgEx.Match($"{r}");
            if (m.Success)
                return GetArgumentsCount(m.Groups[1].Value);
        }

        GeneratorLogger.Add("!!ArgumentsCount value not found!");

        return 0;
    }

    private int GetArgumentsCount(string strValue) => strValue switch
    {
        "One" => 1,
        "Two" => 2,
        "Three" => 3,
        "Four" => 4,
        "Five" => 5,
        "Six" => 6,
        "Seven" => 7,
        _ => 0
    };
}
