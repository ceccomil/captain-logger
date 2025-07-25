namespace CaptainLogger.Tests;

[Collection("Loggers Tests")]
public class JsonCptLoggerTests
{
  [Fact]
  public void CanCreateJsonCptLogger_WithConsoleRecipient()
  {
    // Arrange
    var logger = new JsonCptLogger(
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
    var logger = new JsonCptLogger(
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
    using var doc = JsonDocument.Parse(output);
  }

  [Fact]
  public async Task Log_WithFileRecipient_WritesToFile()
  {
    // Arrange
    await Task.Delay(100); // Ensure any previous logs are flushed
    var logPath = new FileInfo("./TestingLogs/Xunit-JsonCptLogger.log");

    if (logPath.Directory!.Exists)
    {
      logPath.Directory.Delete(recursive: true);
    }

    var logger = new JsonCptLogger(
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
    using var doc = JsonDocument.Parse(content);
  }

  [Fact]
  public void Log_WithException_WritesMessageAndStackTraceToConsole()
  {
    // Arrange
    var logger = new JsonCptLogger(
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
    using var doc = JsonDocument.Parse(output);
    var exObj = doc.RootElement.GetProperty("exception");
    var stackTrace = exObj.GetProperty("stackTrace").GetString();
    Assert.Contains("CptLoggerTests.cs:line 125", stackTrace); // part of the stack trace
  }

  [Fact]
  public async Task ParallelLog_WritesValidJsonLines()
  {
    // Arrange
    LogFileSystem.AllowTestsToCloseLogFile();
    await Task.Delay(100); // let prior test flush

    var logPath = new FileInfo("./TestingLogs/Xunit-ParallelJson.log");

    if (logPath.Directory!.Exists)
    {
      logPath.Directory.Delete(recursive: true);
    }

    var logger = new JsonCptLogger(
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

    const int logCount = 1_000;
    var tasks = Enumerable.Range(0, logCount)
      .Select(x => Task.Run(() =>
      {
        logger.Log(
            logLevel: LogLevel.Information,
            eventId: new EventId(x, $"E{x}"),
            state: $"Log entry {x}",
            exception: null,
            formatter: (s, e) => s.ToString());
      }))
      .ToArray();

    // Act
    await Task.WhenAll(tasks);

    // Flush file writer
    LogFileSystem.AllowTestsToCloseLogFile();

    // Assert
    Assert.True(File.Exists(logPath.FullName));
    var lines = await File.ReadAllLinesAsync(logPath.FullName);
    Assert.Equal(logCount, lines.Length); // Expect one log entry per line

    foreach (var line in lines)
    {
      using var doc = JsonDocument.Parse(line); // Throws if corrupted
      Assert.True(doc.RootElement.TryGetProperty("message", out _));
    }
  }
}
