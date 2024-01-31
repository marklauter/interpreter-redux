namespace Luthor;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1051:Do not declare visible instance fields",
    Justification = "it's a struct")]
public readonly ref struct Token(
    int offset,
    int length,
    Tokens type,
    string symbol)
{
    public readonly int Offset = offset;
    public readonly int Length = length;
    public readonly Tokens Type = type;
    public readonly string Symbol = symbol;
}
