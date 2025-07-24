namespace CaptainLogger.LoggingLogic;

internal readonly struct LogLine(
  DateTime time,
  LogSegment timeStamp,
  LogSegment level,
  LogSegment message,
  LogSegment category,
  LogSegment correlationId,
  LogSegment spacer)
{
  private readonly StringBuilder _content = new();

  public DateTime Time { get; } = time;
  public LogSegment TimeStamp { get; } = timeStamp;
  public LogSegment Level { get; } = level;
  public LogSegment Message { get; } = message;
  public LogSegment Category { get; } = category;
  public LogSegment CorrelationId { get; } = correlationId;
  public LogSegment Spacer { get; } = spacer;

  public StringBuilder Content
  {
    get
    {
      if (_content.Length == 0)
      {
        _content.Append(TimeStamp.Value);
        _content.Append(Level.Value);
        _content.Append(Message.Value);
        _content.Append(CorrelationId.Value);
        _content.Append(Category.Value);
        _content.Append(Spacer.Value);
      }

      return _content;
    }
  }

  public override string ToString() => Content.ToString();
}
