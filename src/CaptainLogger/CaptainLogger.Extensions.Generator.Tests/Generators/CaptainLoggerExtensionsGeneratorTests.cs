namespace CaptainLogger.Extensions.Generator.Tests.Generators;

[Collection("CaptainLoggerExtensionsGenerator Tests")]
public class CaptainLoggerExtensionsGeneratorTests
{
  [Fact]
  public void TestClassGeneration()
  {
    // Arrange
    var compilation = TestsHelpers.CreateCompilation("./Data/Debugging");
    var generator = new CaptainLoggerExtensionsGenerator();
    var driver = CSharpGeneratorDriver.Create(generator);

    // Act
    var results = driver.GenerateAndGetResults(compilation);

    // Assert
    Assert.Single(results.Generated);
    Assert.Empty(results.Diagnostics);
  }
}
