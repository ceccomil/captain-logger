namespace CaptainLogger.Tests;

public class GeneratorTests
{
    private static Compilation CreateCompilation(string source)
            => CSharpCompilation.Create("compilation",
                new[]
                {
                    CSharpSyntaxTree.ParseText(source)
                },
                new[]
                {
                    MetadataReference
                    .CreateFromFile(
                        typeof(Binder)
                        .GetTypeInfo()
                        .Assembly
                        .Location)
                },
                new CSharpCompilationOptions(
                    OutputKind
                    .ConsoleApplication));

    [Fact]
    public void Generator_when_working()
    {
        // Arrange
        var inputClass = CreateCompilation(
            """
            static IServiceCollection AddService(IServiceCollection s) => s
            .Configure<ServiceTestOptions>(x => x.InstanceId = _appId)
            .Configure<CaptainLoggerOptions>(x =>
            {
                x.TimeIsUtc = true;
                x.ArgumentsCount = LogArguments.Three;
                x.Templates.Add(LogArguments.One, "The value: {Arg0}" + "\r\n" + "Example log template");
                x.Templates.Add(LogArguments.Two, "Simple mex for two values ({Val1}, {Val2})");
            })
            .AddScoped<IServiceTest, ServiceTestConsoleGenerator>();
            """);

        var codeGen = new CodeGenerator();
        var driver = CSharpGeneratorDriver
            .Create(codeGen);

        // Act
        driver = (CSharpGeneratorDriver)driver
            .RunGeneratorsAndUpdateCompilation(
            inputClass,
            out var outputCompilation,
            out var diagnostics);

        var results = driver
            .GetRunResult()
            .GeneratedTrees;

        var codes = new List<string>();

        foreach (var r in results)
        {
            codes.Add($"{r}");
        }

        // Assert
        Assert.Empty(diagnostics);

        Assert.NotEmpty(outputCompilation.SyntaxTrees);
    }
}
