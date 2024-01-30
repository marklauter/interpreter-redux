namespace Luthor.Tokens;

public record Token(
    int Offset,
    int Length,
    TokenType Type,
    string Symbol);
