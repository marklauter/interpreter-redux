namespace Luthor.Tokens;

public static class TokenTypeExtensions
{
    public static bool IsNaturalDelimiter(this TokenType type)
    {
        return type.HasFlag(TokenType.NaturalDelimiter);
    }

    public static bool IsSimpleName(this TokenType type)
    {
        return type.HasFlag(TokenType.SimpleName);
    }

    public static bool IsLiteral(this TokenType type)
    {
        return type.HasFlag(TokenType.Literal);
    }

    public static bool IsGlyph(this TokenType type)
    {
        return type.HasFlag(TokenType.Glyph);
    }

    public static bool IsOperator(this TokenType type)
    {
        return type.HasFlag(TokenType.Operator);
    }
}
