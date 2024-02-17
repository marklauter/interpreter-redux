namespace Luthor.Tokens;

public sealed record Token(
    int Offset,
    int Length,
    TokenType Type);


[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1051:Do not declare visible instance fields",
    Justification = "it's a readonly struct, so stfu")]
public readonly ref struct RefToken(
    int line,
    int offset,
    int length,
    int code)
{
    public readonly int Line = line;
    public readonly int Offset = offset;
    public readonly int Length = length;
    public readonly int Code = code;
}
