using System.Runtime.CompilerServices;

namespace Luthor.Context;

public static class MatchResultExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMatch(this MatchResult match)
    {
        return match.Token.Type.IsMatch();
    }
}

