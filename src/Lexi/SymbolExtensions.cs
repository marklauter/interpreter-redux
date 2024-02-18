using System.Runtime.CompilerServices;

namespace Lexi;

public static class SymbolExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMatch(this Symbol symbol)
    {
        return symbol.Token
            is not Tokens.NoMatch
            and not Tokens.LexError;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsError(this Symbol symbol)
    {
        return symbol.Token.HasFlag(Tokens.LexError);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEndOfSource(this Symbol symbol)
    {
        return symbol.Token.HasFlag(Tokens.EndOfSource);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIdentifier(this Symbol symbol)
    {
        return symbol.Token.HasFlag(Tokens.Identifier);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsKeyword(this Symbol symbol)
    {
        return symbol.Token.HasFlag(Tokens.Keyword);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLiteral(this Symbol symbol)
    {
        return symbol.Token.HasFlag(Tokens.Literal);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOperator(this Symbol symbol)
    {
        return symbol.Token.HasFlag(Tokens.Operator);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOpenCircumfixDelimiter(this Symbol symbol)
    {
        return symbol.Token.HasFlag(Tokens.OpenCircumfixDelimiter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCloseCircumfixDelimiter(this Symbol symbol)
    {
        return symbol.Token.HasFlag(Tokens.CloseCircumfixDelimiter);
    }
}
