namespace CaptainLogger.Extensions.Generator.Tests.Generators;

[Collection("CaptainLoggerMessagesGenerator Tests")]
public class CaptainLoggerMessagesGeneratorTests
{
  [Fact]
  public void TestClassGeneration()
  {
    // Arrange
    var compilation = TestsHelpers.CreateCompilation("./Data/Debugging");
    var generator = new CaptainLoggerMessagesGenerator();
    var driver = CSharpGeneratorDriver.Create(generator);

    // Act
    var results = driver.GenerateAndGetResults(compilation);

    // Assert

    Assert.Empty(results.Diagnostics);
  }
}
