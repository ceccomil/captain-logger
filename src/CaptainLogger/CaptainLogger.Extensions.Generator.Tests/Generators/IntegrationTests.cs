namespace CaptainLogger.Extensions.Generator.Tests.Generators;

[Collection("Integration Tests")]
public class IntegrationTests
{
  [Theory]
  [InlineData("Example")]
  [InlineData("Example1")]
  [InlineData("Example2")]
  [InlineData("Example3")]
  public void WhenGeneratingExtensionsAndMessagesShouldCompile(string file)
  {
    // Arrange
    const string sourceFolder = "./Data/IntegrationTests";
    var sourceFile = $"{sourceFolder}/{file}.cs";

    var compilation = TestsHelpers.CreateCompilationFromFile(
      sourceFile,
      isNullable: true);

    IIncrementalGenerator[] generators =
    [
      new CaptainLoggerMessagesGenerator(),
      new CaptainLoggerExtensionsGenerator()
    ];

    var driver = CSharpGeneratorDriver.Create(generators);

    // Act
    var results = driver.GenerateAndGetResults(compilation);

    var compilationResult = TestsHelpers.GetDiagnosticsForGenerated(
        sourceFolder,
        results.Generated);

    // Assert
    Assert.Empty(results.Diagnostics);
    Assert.Empty(compilationResult.Errors);
  }
}
