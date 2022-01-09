
namespace CaptainLogger.Logic;

internal struct RowPart
{
    internal string Value { get; }
    internal ConsoleColor Color { get; }

    internal RowPart(
        string value,
        ConsoleColor color)
    {
        Value = value;
        Color = color;
    }

    public override string ToString() => Value;
}
