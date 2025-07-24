namespace CaptainLogger.Tests;

[Collection("Loggers Tests")]
public class CptLoggerTests
{
  [Fact]
  public void CanCreateCptLogger_WithConsoleRecipient()
  {
    // Arrange
    var logger = new CptLogger(
      category: "TestCategory",
      provider: "TestProvider",
      getCurrentConfig: () => new CaptainLoggerOptions
      {
        LogRecipients = Recipients.Console
      },
      getCurrentFilters: () => new LoggerFilterOptions(),
      onLogEntry: args => Task.CompletedTask
    );

    Assert.NotNull(logger); // just confirms creation
  }

  [Fact]
  public void Log_WithConsoleRecipient_WritesToConsole()
  {
    // Arrange
    var logger = new CptLogger(
      category: "TestCategory",
      provider: "TestProvider",
      getCurrentConfig: () => new CaptainLoggerOptions
      {
        LogRecipients = Recipients.Console
      },
      getCurrentFilters: () => new LoggerFilterOptions(),
      onLogEntry: args => Task.CompletedTask
    );

    using var sw = new StringWriter();
    Console.SetOut(sw); // Redirect console output

    // Act
    logger.Log(
      logLevel: LogLevel.Information,
      eventId: new EventId(1, "Test"),
      state: "Hello world",
      exception: null,
      formatter: (s, e) => s.ToString());

    // Flush (Log is sync wrapper, so no await needed)
    Console.Out.Flush();

    // Assert
    var output = sw.ToString();
    Assert.Contains("Hello world", output);
  }

  [Fact]
  public async Task Log_WithFileRecipient_WritesToFile()
  {
    // Arrange
    await Task.Delay(100); // Ensure any previous logs are flushed
    var logPath = new FileInfo("./TestingLogs/Xunit-CptLogger.log");

    if (logPath.Directory!.Exists)
    {
      logPath.Directory.Delete(recursive: true);
    }

    var logger = new CptLogger(
      category: "TestCategory",
      provider: "TestProvider",
      getCurrentConfig: () => new CaptainLoggerOptions
      {
        LogRecipients = Recipients.File,
        FilePath = logPath.FullName,
        FileRotation = LogRotation.None
      },
      getCurrentFilters: () => new LoggerFilterOptions(),
      onLogEntry: args => Task.CompletedTask
    );

    // Act
    logger.Log(
      logLevel: LogLevel.Warning,
      eventId: new EventId(2, "TestWarning"),
      state: "This should go in a file",
      exception: null,
      formatter: (s, e) => s.ToString());

    // Allow time for async write
    await Task.Delay(100); // crude but avoids File.ReadAllText race

    // Assert
    LogFileSystem.AllowTestsToCloseLogFile();
    Assert.True(File.Exists(logPath.FullName));
    var content = await File.ReadAllTextAsync(logPath.FullName);
    Assert.Contains("This should go in a file", content);
  }

  [Fact]
  public void Log_WithException_WritesMessageAndStackTraceToConsole()
  {
    // Arrange
    var logger = new CptLogger(
      category: "TestCategory",
      provider: "TestProvider",
      getCurrentConfig: () => new CaptainLoggerOptions
      {
        LogRecipients = Recipients.Console
      },
      getCurrentFilters: () => new LoggerFilterOptions(),
      onLogEntry: args => Task.CompletedTask
    );

    using var sw = new StringWriter();
    Console.SetOut(sw);

    Exception? ex = null;

    try
    {
      throw new InvalidOperationException("Something went wrong");
    }
    catch (InvalidOperationException exception)
    {
      ex = exception;
    }

    // Act
    logger.Log(
      logLevel: LogLevel.Error,
      eventId: new EventId(3, "ErrorEvent"),
      state: "Logging with exception",
      exception: ex,
      formatter: (s, e) => s.ToString());

    Console.Out.Flush();

    // Assert
    var output = sw.ToString();
    Assert.Contains("Something went wrong", output);
    Assert.Contains("CptLoggerTests.cs:line 123", output); // part of the stack trace
  }
}
