namespace CaptainLogger.Extensions.Generator.Tests.Helpers;

internal static class TestsHelpers
{
  private static SyntaxTree GetTree(string filePath)
  {
    var fi = new FileInfo(filePath);

    if (!fi.Exists)
    {
      throw new InvalidOperationException(
        $"File not found: {filePath}");
    }

    var content = File.ReadAllText(fi.FullName);

    var relativePath = Path
      .GetRelativePath(fi.Directory!.FullName, fi.FullName)
      .Replace("\\", "/");

    return CSharpSyntaxTree.ParseText(content, path: relativePath);
  }

  private static List<PortableExecutableReference> GetMetadataReferences()
  {
    var refs = new List<PortableExecutableReference>();

    var currentAssembly = typeof(TestsHelpers)
      .GetTypeInfo()
      .Assembly;

    var context = DependencyContext.Load(currentAssembly)
      ?? throw new InvalidOperationException("Cannot load dependencies context!");

    var libraries = context.RuntimeLibraries
      .SelectMany(x => x.GetDefaultAssemblyNames(context))
      .Select(Assembly.Load)
      .Select(x => x.Location)
      .Distinct()
      .ToList();

    foreach (var library in libraries.Where(library => refs.All(x => x.FilePath != library)))
    {
      refs.Add(MetadataReference.CreateFromFile(library));
    }

    return refs;
  }

  private static CSharpCompilation Create(
    IEnumerable<SyntaxTree> trees,
    bool isNullable)
  {
    var options = new CSharpCompilationOptions(
      OutputKind.ConsoleApplication);

    if (isNullable)
    {
      options = options.WithNullableContextOptions(
        NullableContextOptions.Enable);
    }

    return CSharpCompilation.Create(
      "compilation",
      trees,
      [
        MetadataReference.CreateFromFile(
          typeof(Assembly)
          .GetTypeInfo()
          .Assembly
          .Location)
      ],
      options);
  }

  private static List<SyntaxTree> GetTrees(string? sourceFolder)
  {
    List<SyntaxTree> trees = [];

    if (string.IsNullOrWhiteSpace(sourceFolder))
    {
      return trees;
    }

    trees.AddRange(Directory.EnumerateFiles(
      sourceFolder,
      "*.cs*",
      SearchOption.AllDirectories)
      .Select(GetTree));

    return trees;
  }

  public static CSharpCompilation CreateCompilation(
    string? sourceFolder,
    bool isNullable = false)
  {
    var trees = GetTrees(sourceFolder);
    return Create(trees, isNullable);
  }

  public static CSharpCompilation CreateCompilationFromFile(
    string filePath,
    bool isNullable = false)
  {
    var tree = GetTree(filePath);
    return Create([tree], isNullable);
  }

  public static EnrichedDiagnostics GetDiagnosticsForGenerated(
    string? sourceFolder,
    IEnumerable<GeneratedFile> generatedFiles)
  {
    var refs = GetMetadataReferences();

    var trees = GetTrees(sourceFolder);
    trees.AddRange(generatedFiles.Select(gf =>
      CSharpSyntaxTree.ParseText(gf.Content, path: gf.Name)));

    var compilation = CSharpCompilation.Create(
      "resulting_compilation",
      trees,
      refs,
      new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    var diags = new EnrichedDiagnostics(compilation);

    return diags;
  }

  public static GenResults GenerateAndGetResults(
    this GeneratorDriver driver,
    Compilation inCompilation)
  {
    var csDriver = (CSharpGeneratorDriver)driver
      .RunGeneratorsAndUpdateCompilation(
        inCompilation,
        out var outCompilation,
        out var diagnostics);

    var results = csDriver
      .GetRunResult()
      .GeneratedTrees
      .Select(x => new GeneratedFile(x.FilePath, x.ToString()))
      .ToList();

    return (results, outCompilation, diagnostics);
  }
}

internal record GeneratedFile(string Name, string Content);
