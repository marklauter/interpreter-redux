namespace Luthor.Tokens;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1051:Do not declare visible instance fields",
    Justification = "it's a struct")]
public readonly ref struct RefToken(
    int offset,
    int length,
    TokenType type,
    string value)
{
    public readonly int Offset = offset;
    public readonly int Length = length;
    public readonly TokenType Type = type;
    public readonly string Value = value;
}
