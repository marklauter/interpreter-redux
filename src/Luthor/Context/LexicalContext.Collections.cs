using System.Collections;
using System.Runtime.CompilerServices;

namespace Luthor.Context;

public sealed partial class LexicalContext
    : IEnumerable<TokenMatcher>
{
    public int Length => matchers.Length;

    public TokenMatcher this[int i] => matchers[i];

    public TokenMatcher this[Tokens key] => map[key];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<TokenMatcher> AsReadOnlySpan()
    {
        return matchers.AsSpan();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<TokenMatcher> GetEnumerator()
    {
        return (IEnumerator<TokenMatcher>)matchers.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
