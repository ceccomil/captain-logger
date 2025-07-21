namespace CaptainLogger.LoggingLogic;

internal readonly struct RowParts(
  DateTime time,
  RowPart timeStamp,
  RowPart level,
  RowPart message,
  RowPart category,
  RowPart spacer)
{
  private readonly StringBuilder _content = new();

  public DateTime Time { get; } = time;
  public RowPart TimeStamp { get; } = timeStamp;
  public RowPart Level { get; } = level;
  public RowPart Message { get; } = message;
  public RowPart Category { get; } = category;
  public RowPart Spacer { get; } = spacer;

  public StringBuilder Content
  {
    get
    {
      if (_content.Length == 0)
      {
        _content.Append(TimeStamp.Value);
        _content.Append(Level.Value);
        _content.Append(Message.Value);
        _content.Append(Category.Value);
        _content.Append(Spacer.Value);
      }

      return _content;
    }
  }

  public override string ToString() => Content.ToString();
}
