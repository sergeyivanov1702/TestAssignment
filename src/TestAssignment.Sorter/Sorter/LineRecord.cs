namespace TestAssignment.Sorter;

public readonly struct LineRecord : IComparable<LineRecord>
{
    private const string Delimiter = ". ";
    private readonly int _textStartIndex;

    private LineRecord(int number, string originalLine, int textStartIndex)
    {
        Number = number;
        OriginalLine = originalLine;
        _textStartIndex = textStartIndex;
    }

    public int Number { get; }
    public string OriginalLine { get; }
    private readonly ReadOnlySpan<char> TextSpan => OriginalLine.AsSpan(_textStartIndex);

    public int CompareTo(LineRecord other)
    {
        int cmp = TextSpan.CompareTo(other.TextSpan, StringComparison.Ordinal);
        return cmp != 0 ? cmp : Number.CompareTo(other.Number);
    }

    public static LineRecord Parse(string line)
    {
        var span = line.AsSpan();
        int idx = span.IndexOf(Delimiter, StringComparison.Ordinal);

        var numPart = span[..idx];
        var number = int.Parse(numPart);

        return new LineRecord(number, line, idx + Delimiter.Length);
    }
}
