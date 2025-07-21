namespace CaptainLogger.Helpers;

internal static class LogFileSystem
{
  private static FileInfo? _currentLog;
  private static string _timeSuffix = "";

  private static FileStream? _inProcessLogFile;
  private static StreamWriter? _inProcessLogWriter;

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

  private static void CloseAndDispose(this Stream? stream)
  {
    if (stream is null)
    {
      return;
    }

    stream.Close();
    stream.Dispose();
  }

  private static void CloseAndDispose(this TextWriter? stream)
  {
    if (stream is null)
    {
      return;
    }

    stream.Close();
    stream.Dispose();
  }

  private static FileInfo InitAndLock(this FileInfo file)
  {
    _inProcessLogFile = new FileStream(file.FullName, FileMode.OpenOrCreate, FileAccess.Write);
    _inProcessLogFile.Position = _inProcessLogFile.Length;
    _inProcessLogWriter = new StreamWriter(_inProcessLogFile, Encoding.UTF8);

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
    _inProcessLogWriter.CloseAndDispose();

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

  public static async Task WriteToLogFile(
    this LogLine row,
    CaptainLoggerOptions config,
    CancellationToken cancellationToken)
  {
    config.CheckLogFileName(row.Time);

    if (_inProcessLogWriter is null || _inProcessLogFile is null)
    {
      throw new InvalidOperationException("Log filestream must be valid!");
    }

    await _inProcessLogWriter.WriteAsync(row.Content, cancellationToken);

    _inProcessLogWriter.Flush();
    _inProcessLogFile.Flush();
  }
}
