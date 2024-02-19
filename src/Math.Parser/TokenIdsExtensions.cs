using System.Runtime.CompilerServices;

namespace Math.Parser;

internal static class TokenIdsExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFactor(this int tokenId)
    {
        return tokenId switch
        {
            TokenIds.MULTIPLY => true,
            TokenIds.DIVIDE => true,
            TokenIds.MODULUS => true,
            _ => false,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsTerm(this int tokenId)
    {
        return tokenId switch
        {
            TokenIds.ADD => true,
            TokenIds.SUBTRACT => true,
            _ => false,
        };
    }
}
