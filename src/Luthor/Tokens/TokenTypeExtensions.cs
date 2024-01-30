using System.Runtime.CompilerServices;

namespace Luthor.Tokens;

public static class TokenTypeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDelimiter(this TokenType type)
    {
        return type.HasFlag(TokenType.Delimiter);
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
