namespace CaptainLogger.Tests;

[Collection("Loggers Tests")]
public class LogFileSystemFlushTests
{
  private static CptLogger CreateFileLogger(string fullPath) => new(
    category: "TestCategory",
    provider: "TestProvider",
    getCurrentConfig: () => new CaptainLoggerOptions
    {
      LogRecipients = Recipients.File,
      FilePath = fullPath,
      FileRotation = LogRotation.None
    },
    getCurrentFilters: () => new LoggerFilterOptions(),
    onLogEntry: _ => Task.CompletedTask,
    new LoggerExternalScopeProvider()
  );

  private static SemaphoreSlim GetFileGate()
  {
    var field = typeof(LogFileSystem).GetField(
      "_fileGate",
      BindingFlags.NonPublic | BindingFlags.Static);

    Assert.NotNull(field);

    var gate = field!.GetValue(null) as SemaphoreSlim;

    Assert.NotNull(gate);

    return gate!;
  }

  [Fact]
  public async Task FlushLogFileAsync_WaitsForInFlightFileOperation()
  {
    LogFileSystem.AllowTestsToCloseLogFile();

    var logPath = new FileInfo($"./TestingLogs/Xunit-FlushTest-{Guid.NewGuid():N}.log");
    logPath.Directory!.Create();
    if (logPath.Exists)
    {
      logPath.Delete();
    }

    var logger = CreateFileLogger(logPath.FullName);
    logger.Log(
      logLevel: LogLevel.Information,
      eventId: new EventId(1, "FlushTest"),
      state: "flush-test",
      exception: null,
      formatter: (s, _) => s.ToString());

    var gate = GetFileGate();

    await gate.WaitAsync();

    try
    {
      var flushTask = LogFileSystem.FlushLogFileAsync();

      var completed = await Task.WhenAny(flushTask, Task.Delay(50));
      Assert.NotSame(flushTask, completed);

      gate.Release();

      await flushTask.WaitAsync(TimeSpan.FromSeconds(2));
    }
    finally
    {
      // Safety: if the test failed before releasing, make sure we don't block the suite.
      if (gate.CurrentCount == 0)
      {
        gate.Release();
      }

      LogFileSystem.AllowTestsToCloseLogFile();
    }
  }

  [Fact]
  public async Task FlushLogFileAsync_RespectsCancellationWhileWaitingForInFlightFileOperation()
  {
    var gate = GetFileGate();

    await gate.WaitAsync();

    try
    {
      using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

      await Assert.ThrowsAnyAsync<OperationCanceledException>(
        () => LogFileSystem.FlushLogFileAsync(cts.Token));
    }
    finally
    {
      if (gate.CurrentCount == 0)
      {
        gate.Release();
      }
    }
  }

  [Fact]
  public async Task FlushLogFile_WaitsForInFlightFileOperation()
  {
    LogFileSystem.AllowTestsToCloseLogFile();

    var logPath = new FileInfo($"./TestingLogs/Xunit-FlushTest-{Guid.NewGuid():N}.log");
    logPath.Directory!.Create();
    if (logPath.Exists)
    {
      logPath.Delete();
    }

    var logger = CreateFileLogger(logPath.FullName);
    logger.Log(
      logLevel: LogLevel.Information,
      eventId: new EventId(1, "FlushTest"),
      state: "flush-test",
      exception: null,
      formatter: (s, _) => s.ToString());

    var gate = GetFileGate();

    await gate.WaitAsync();

    try
    {
      var flushTask = Task.Run(LogFileSystem.FlushLogFile);

      var completed = await Task.WhenAny(flushTask, Task.Delay(50));
      Assert.NotSame(flushTask, completed);

      gate.Release();

      await flushTask.WaitAsync(TimeSpan.FromSeconds(2));
    }
    finally
    {
      if (gate.CurrentCount == 0)
      {
        gate.Release();
      }

      LogFileSystem.AllowTestsToCloseLogFile();
    }
  }
}
