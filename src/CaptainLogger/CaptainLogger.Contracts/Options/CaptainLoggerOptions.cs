namespace CaptainLogger.Contracts.Options;

/// <summary>
/// Options used to configure behavior for <c>CaptainLogger</c>.
/// </summary>
public class CaptainLoggerOptions : IDisposable
{
  internal static string DefaultLogName { get; } =
    $"./Logs/{AppDomain.CurrentDomain.FriendlyName.Replace(".", "-")}.log";

  private bool _disposed;

  /// <summary>
  /// Default Color used for messages written to the console.
  /// </summary>
  public ConsoleColor DefaultColor { get; set; } = Console.ForegroundColor;

  /// <summary>
  /// If <c>true</c>, timestamps in log entries will be in UTC. Otherwise, local time is used.
  /// </summary>
  public bool TimeIsUtc { get; set; }

  /// <summary>
  /// If <c>true</c>, a provided exception will not be appended to the log message output.
  /// <para>Does not affect the behavior of the <c>formatter</c> function used by <see cref="ILogger.Log{TState}"/>.</para>
  /// </summary>
  public bool DoNotAppendException { get; set; }

  /// <summary>
  /// Specifies where log entries should be written.
  /// <para>Defaults to <see cref="Recipients.Console"/> and <see cref="Recipients.File"/>.</para>
  /// </summary>
  public Recipients LogRecipients { get; set; } = Recipients.Console | Recipients.File;

  /// <summary>
  /// Path of the log file to write to.
  /// <para>Defaults to <c>./Logs/AssemblyName.log</c>.</para>
  /// </summary>
  public string FilePath { get; set; } = DefaultLogName;

  /// <summary>
  /// Determines how frequently a new log file is created by appending a timestamp to the filename.
  /// <para>Defaults to <see cref="LogRotation.Hour"/>.</para>
  /// </summary>
  public LogRotation FileRotation { get; set; } = LogRotation.Hour;

  /// <summary>
  /// Specifies how many structured logging extension methods should be generated.
  /// <para>
  /// For example, <see cref="LogArguments.One"/> generates:
  /// <c>InformationLog&lt;T0&gt;(this ICaptainLogger, T0 arg0)</c>
  /// </para>
  /// <para>Requires the <c>CaptainLogger.Extensions.Generator</c> package.</para>
  /// </summary>
  public LogArguments ArgumentsCount { get; set; } = LogArguments.Zero;

  /// <summary>
  /// Custom templates for each argument count, used by the generated logging methods.
  /// <para>
  /// For example, the template for <see cref="LogArguments.One"/> might be:
  /// <c>"This is the log template used for one argument: {Arg0}"</c>
  /// </para>
  /// </summary>
  public Dictionary<LogArguments, string> Templates { get; } = [];

  /// <summary>
  /// An optional stream used for logging when <see cref="LogRecipients"/> includes <see cref="Recipients.Stream"/>.
  /// </summary>
  public Stream? LoggerBuffer { get; set; }

  /// <summary>
  /// Indicates whether asynchronous log-related events should be triggered.
  /// <para>When set to <c>false</c>, those events are disabled entirely. Defaults to <c>false</c>.</para>
  /// </summary>
  public bool TriggerAsyncEvents { get; set; }

  /// <summary>
  /// A list of event IDs to suppress from being logged.
  /// Any log entry with a matching ID will be ignored, even if it passes the configured log level filters.
  /// </summary>
  public IEnumerable<int> ExcludedEventIds { get; set; } = [];

  /// <summary>
  /// Gets or sets a value indicating whether high-performance structured logging is enabled.
  /// </summary>
  /// <remarks>
  /// When enabled, this mode is optimized for high-volume logging scenarios.
  /// It alters how <c>CaptainLogger.Extensions.Generator</c> interprets the <see cref="CaptainLoggerOptions.Templates"/> dictionary.
  /// For example, if the two-argument template is defined as:
  /// <c>This is my {baseUrl} and this is my route {route}</c>,
  /// the generator will emit a structured template like:
  /// <c>"baseUrl":"{BaseUrl}","route":"{Route}"</c>
  /// and the final log output will be a well-formed JSON object.
  /// </remarks>
  public bool HighPerfStructuredLogging { get; set; } = false;

  /// <summary>
  /// A dictionary of additional static properties to include with every structured log entry,
  /// such as <c>service-name</c>, <c>deployment-env</c>, or other metadata useful for log aggregation and filtering.
  /// These key-value pairs will be added to the root of each structured JSON log object.
  /// </summary>
  public Dictionary<string, object> StructuredLogMetadata { get; } = [];

  /// <summary>
  /// Disposes the logger options and any associated resources, such as the <see cref="LoggerBuffer"/>.
  /// </summary>
  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  private void Dispose(bool disposing)
  {
    if (_disposed || !disposing)
    {
      return;
    }

    if (LoggerBuffer is not null)
    {
      LoggerBuffer.Close();
      LoggerBuffer.Dispose();
      LoggerBuffer = null;
    }

    _disposed = true;
  }
}
