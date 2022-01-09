namespace CaptainLogger.Logic;

internal class RowParts
{
    internal DateTime Time { get; set; }
    internal RowPart TimeStamp { get; set; }
    internal RowPart Level { get; set; }
    internal RowPart Message { get; set; }
    internal RowPart Category { get; set; }
    internal RowPart Spacer { get; set; }

    public override string ToString() => $"{TimeStamp}{Level}{Message}{Category}{Spacer}";
}
