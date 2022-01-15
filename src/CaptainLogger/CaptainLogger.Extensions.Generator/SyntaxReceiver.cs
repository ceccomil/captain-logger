namespace CaptainLogger.Extensions.Generator;

public class SyntaxReceiver : ISyntaxContextReceiver
{
    public List<string> GeneratorLogger { get; } = new();
    public int ArgumentsCount { get; private set; } = 0;
    public string[] Templates { get; private set; } = Array.Empty<string>();

    private bool _configFound = false;

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (!_configFound &&
            context.Node is InvocationExpressionSyntax ies &&
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
            _configFound = ArgumentsCount > 0;
            Templates = GetTemplates(lambda);
            GeneratorLogger.Add($"ArgumentsCount: {ArgumentsCount}");

            foreach (var t in Templates)
                GeneratorLogger.Add($"Tpl -> {t}");
        }
    }

    private string[] GetTemplates(LambdaExpressionSyntax lambda)
    {
        if (ArgumentsCount <= 0)
            return Array.Empty<string>();

        var tpls = new string[ArgumentsCount];

        var rows = lambda
            .DescendantNodes()
            .OfType<ExpressionStatementSyntax>();

        for (int i = 0; i < ArgumentsCount; i++)
            tpls[i] = GetTemplate(i + 1, rows);            

        return tpls;
    }

    private string GetTemplate(int arguments, IEnumerable<ExpressionStatementSyntax> rows)
    {
        var rgEx = new Regex(@$"^.*Templates\.Add\(.*LogArguments\.{GetArgumentsCount(arguments)},(.+)\);$",
             RegexOptions.IgnoreCase | RegexOptions.Singleline);

        foreach (var r in rows)
        {
            var m = rgEx.Match($"{r}");

            if (m.Success)
                return m.Groups[1].Value;
        }

        return GetTemplateString(arguments);
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

    private string GetArgumentsCount(int value) => value switch
    {
        1 => "One",
        2 => "Two",
        3 => "Three",
        4 => "Four",
        5 => "Five",
        6 => "Six",
        7 => "Seven",
        _ => ""
    };
}
