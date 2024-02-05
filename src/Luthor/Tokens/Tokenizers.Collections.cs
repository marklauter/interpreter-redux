using System.Runtime.CompilerServices;

namespace Luthor.Tokens;

public sealed partial class Tokenizers
{
    public int Length => patternMatchers.Length;

    public PatternMatcher this[int i] => patternMatchers[i];

    public PatternMatcher this[TokenType key] => map[key];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<PatternMatcher> AsReadOnlySpan()
    {
        return patternMatchers.AsSpan();
    }
}
