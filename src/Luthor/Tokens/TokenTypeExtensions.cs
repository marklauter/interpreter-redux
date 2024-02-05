using System.Runtime.CompilerServices;

namespace Luthor.Tokens;

public static class TokenTypeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNoMatch(this TokenType type)
    {
        return type.HasFlag(TokenType.NoMatch);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMatch(this TokenType type)
    {
        return type != TokenType.Error
            && !type.HasFlag(TokenType.NoMatch);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDelimiter(this TokenType type)
    {
        return type.HasFlag(TokenType.Delimiter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhitespace(this TokenType type)
    {
        return type.HasFlag(TokenType.Whitespace);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCircumfixDelimiter(this TokenType type)
    {
        return type.HasFlag(TokenType.CircumfixDelimiter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsName(this TokenType type)
    {
        return type.HasFlag(TokenType.Name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLiteral(this TokenType type)
    {
        return type.HasFlag(TokenType.Literal);
    }
}
