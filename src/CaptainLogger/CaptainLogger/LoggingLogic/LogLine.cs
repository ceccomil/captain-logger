namespace CaptainLogger.LoggingLogic;

internal sealed partial class LogLine(
  DateTime time,
  LogSegment timeStamp,
  LogSegment level,
  LogSegment message,
  LogSegment category,
  LogSegment correlationId,
  LogSegment scopedValues,
  LogSegment spacer) : IDisposable
{
  private StringBuilder? _builder;
  private string? _cachedLine;
  private bool _disposed;

  private static readonly Regex _ansiRegex = AnsiRegex();

  public DateTime Time { get; } = time;
  public LogSegment TimeStamp { get; } = timeStamp;
  public LogSegment Level { get; } = level;
  public LogSegment Message { get; } = message;
  public LogSegment Category { get; } = category;
  public LogSegment CorrelationId { get; } = correlationId;
  public LogSegment ScopedValues { get; } = scopedValues;
  public LogSegment Spacer { get; } = spacer;
  public int LineLength { get; } =
    timeStamp.Value.Length +
    level.Value.Length +
    message.Value.Length +
    category.Value.Length +
    correlationId.Value.Length +
    spacer.Value.Length;

  public override string ToString()
  {
    return ToString(removeAnsiCodes: false);
  }

  public string ToString(bool removeAnsiCodes)
  {
    ObjectDisposedException.ThrowIf(_disposed, typeof(LogLine));

    if (_cachedLine is not null)
    {
      return _cachedLine;
    }

    if (_builder is null)
    {
      Build(removeAnsiCodes);
    }

    _cachedLine = StringBuilderCache.GetStringAndCacheIt(_builder!);
    _builder = null;
    return _cachedLine;
  }

  public ReadOnlySpan<char> AsSpan(bool removeAnsiCodes) => ToString(removeAnsiCodes)
    .AsSpan();

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

    if (_builder is not null)
    {
      StringBuilderCache.StoreForReuse(_builder);
      _builder = null;
    }

    _cachedLine = null;
    _disposed = true;
  }

  private void Build(bool removeAnsiCodes)
  {
    _builder = StringBuilderCache.GetNewOrCached(LineLength);

    _builder
      .Append(TimeStamp.Value)
      .Append(Level.Value);

    AppendMessage(removeAnsiCodes);

    _builder
      .Append(CorrelationId.Value)
      .Append(ScopedValues.Value)
      .Append(Category.Value)
      .Append(Spacer.Value);
  }

  private void AppendMessage(bool removeAnsiCodes)
  {
    if (!removeAnsiCodes)
    {
      _builder!.Append(Message.Value);
      return;
    }

    _builder!.Append(_ansiRegex.Replace(
      Message.Value, 
      string.Empty));
  }

  [GeneratedRegex(
    @"\x1B\[[0-?]*[ -/]*[@-~]|\x1B\][^\x07]*(?:\x07|\x1B\\)|\x1B[@-Z\\-_]", 
    RegexOptions.Compiled)]
  private static partial Regex AnsiRegex();
}
