using System.Runtime.CompilerServices;

namespace Luthor;

public static class TokensExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNoMatch(this Tokens type)
    {
        return type.HasFlag(Tokens.NoMatch);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMatch(this Tokens type)
    {
        return type != Tokens.Error
            && !type.HasFlag(Tokens.NoMatch);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDelimiter(this Tokens type)
    {
        return type.HasFlag(Tokens.Delimiter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCircumfixDelimiter(this Tokens type)
    {
        return type.HasFlag(Tokens.CircumfixDelimiter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsName(this Tokens type)
    {
        return type.HasFlag(Tokens.Name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLiteral(this Tokens type)
    {
        return type.HasFlag(Tokens.Literal);
    }
}
