using System.Runtime.CompilerServices;

namespace Luthor.Tokens;

public static class TokenExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNoMatch(this Token token)
    {
        return token.Type.IsNoMatch();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMatch(this Token token)
    {
        return token.Type.IsMatch();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsError(this Token token)
    {
        return token.Type == TokenType.Error;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhiteSpace(this Token token)
    {
        return token.Type.IsWhitespace();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEndOfSource(this Token token)
    {
        return token.Type.HasFlag(TokenType.EndOfSource);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNumber(this Token token)
    {
        return token.Type.HasFlag(TokenType.NumericLiteral);
    }
}
