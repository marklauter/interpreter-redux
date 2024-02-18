namespace Luthor.Tokens;

public sealed record Token(
    int Offset,
    int Length,
    TokenType Type);
