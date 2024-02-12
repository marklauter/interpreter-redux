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
    public static bool IsName(this Token token)
    {
        return token.Type.HasFlag(TokenType.Name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIdentifier(this Token token)
    {
        return token.Type.HasFlag(TokenType.Identifier);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsReservedWord(this Token token)
    {
        return token.Type.HasFlag(TokenType.ReservedWord);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLiteral(this Token token)
    {
        return token.Type.HasFlag(TokenType.Literal);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNumber(this Token token)
    {
        return token.Type.HasFlag(TokenType.NumericLiteral);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsString(this Token token)
    {
        return token.Type.HasFlag(TokenType.StringLiteral);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBoolean(this Token token)
    {
        return token.Type.HasFlag(TokenType.BooleanLiteral);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCharacter(this Token token)
    {
        return token.Type.HasFlag(TokenType.CharacterLiteral);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOperator(this Token token)
    {
        return token.Type.HasFlag(TokenType.Operator);
    }

}
