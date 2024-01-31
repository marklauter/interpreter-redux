using System.Collections;
using System.Runtime.CompilerServices;

namespace Luthor.Context;

public sealed partial class LexicalContext
    : IEnumerable<TokenReader>
{
    public int Length => readers.Length;

    public TokenReader this[int i] => readers[i];

    public TokenReader this[Tokens key] => map[key];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<TokenReader> AsReadOnlySpan()
    {
        return readers.AsSpan();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<TokenReader> GetEnumerator()
    {
        return (IEnumerator<TokenReader>)readers.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
