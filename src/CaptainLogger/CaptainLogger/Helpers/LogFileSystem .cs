namespace CaptainLogger.Helpers;

internal static class LogFileSystem
{
  private static long _lastFlushTicks = Stopwatch.GetTimestamp();

  private static FileInfo? _currentLog;
  private static string _timeSuffix = "";

  private static FileStream? _inProcessLogFile;

  public static FileInfo CurrentLog
  {
    get
    {
      if (_currentLog is null)
      {
        throw new InvalidOperationException(
          "Current log file must be valid!");
      }

      return _currentLog;
    }
  }

  public static void AllowTestsToCloseLogFile()
  {
    _inProcessLogFile?.CloseAndDispose();
    _currentLog = null;
    _timeSuffix = "";
  }

  public static async Task WriteToLogFile(
    this ArrayBufferWriter<byte> line,
    DateTime logTime,
    CaptainLoggerOptions config)
  {
    config.CheckLogFileName(logTime);

    if (_inProcessLogFile is null)
    {
      throw new InvalidOperationException("Log filestream must be valid!");
    }

    await _inProcessLogFile.WriteAsync(line.WrittenMemory);

    MaybeFlushFile();
  }

  public static async Task WriteToLogFile(
    this LogLine line,
    CaptainLoggerOptions config)
  {
    config.CheckLogFileName(line.Time);

    if (_inProcessLogFile is null)
    {
      throw new InvalidOperationException("Log filestream must be valid!");
    }

    var data = line.AsSpan(config.RemoveAnsiCodes);
    var byteCount = Encoding.UTF8.GetByteCount(data);
    var rented = ArrayPool<byte>.Shared.Rent(byteCount);
    var written = Encoding.UTF8.GetBytes(data, rented);
    await _inProcessLogFile.WriteAsync(rented.AsMemory(0, written));
    ArrayPool<byte>.Shared.Return(rented);

    MaybeFlushFile();
  }

  public static void FlushLogFile()
  {
    _inProcessLogFile?.Flush();
    _lastFlushTicks = Stopwatch.GetTimestamp();
  }

  private static void CloseAndDispose(this Stream? stream)
  {
    if (stream is null)
    {
      return;
    }

    try
    {
      stream.Close();
      stream.Dispose();
    }
    catch (ObjectDisposedException)
    {
      // Ignore if already disposed
    }
  }

  private static FileInfo InitAndLock(this FileInfo file)
  {
    _inProcessLogFile = new FileStream(file.FullName, FileMode.OpenOrCreate, FileAccess.Write);
    _inProcessLogFile.Position = _inProcessLogFile.Length;

    return file;
  }

  private static void CheckLogFileName(
    this CaptainLoggerOptions config,
    DateTime time,
    int? counter = default)
  {
    var tSuffix = config.FileRotation.GetTimeSuffix(time);

    if (_currentLog is not null && _timeSuffix == tSuffix)
    {
      return;
    }

    _timeSuffix = tSuffix;
    var log = config.GetLogFile(time, counter);

    _inProcessLogFile.CloseAndDispose();

    try
    {
      _currentLog = log.InitAndLock();
    }
    catch
    {
      CheckLogFileName(
        config,
        time,
        counter.GetValueOrDefault() + 1);
    }
  }

  private static FileInfo GetLogFile(
    this CaptainLoggerOptions options,
    DateTime time,
    int? counter = default) => options
      .FilePath
      .GetLogFile(options.FileRotation, time, counter);

  private static FileInfo GetLogFile(
    this string filePath,
    LogRotation fileRotation,
    DateTime time,
    int? counter = default)
  {
    var fullPath = Path.GetFullPath(filePath);
    var dirPath = Path.GetDirectoryName(fullPath);

    if (string.IsNullOrWhiteSpace(dirPath))
    {
      dirPath = Path.GetFullPath("./Logs");
    }

    var file = Path.GetFileNameWithoutExtension(fullPath);
    var ext = Path.GetExtension(fullPath);

    if (!Directory.Exists(dirPath))
    {
      Directory.CreateDirectory(dirPath);
    }

    if (string.IsNullOrWhiteSpace(ext) || ext == ".")
    {
      ext = ".log";
    }

    var fileNoExt = Path.Combine(dirPath, $"{file}{fileRotation.GetTimeSuffix(time)}");

    if (counter.GetValueOrDefault() > 0)
    {
      fileNoExt += $"_{counter:000}";
    }

    return new FileInfo($"{fileNoExt}{ext}");
  }

  private static string GetTimeSuffix(
    this LogRotation fileRotation,
    DateTime time) => fileRotation switch
    {
      LogRotation.Year => $"-{time:yyyy}",
      LogRotation.Month => $"-{time:yyyyMM}",
      LogRotation.Day => $"-{time:yyyyMMdd}",
      LogRotation.Hour => $"-{time:yyyyMMddHH}",
      LogRotation.Minute => $"-{time:yyyyMMddHHmm}",
      _ => ""
    };

  private static void MaybeFlushFile()
  {
    var now = Stopwatch.GetTimestamp();
    var prev = _lastFlushTicks;

    if (now - prev < FlushIntervalTicks)
    {
      return;
    }

    _inProcessLogFile!.Flush();
    _lastFlushTicks = now;
  }
}
