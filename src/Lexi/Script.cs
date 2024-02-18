using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Lexi;

[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "it's a struct")]
public readonly ref struct Script(
    string source,
    int offset,
    int line)
{
    public Script(string source)
        : this(source, 0, 0)
    {
    }

    public readonly string Source = source;
    public readonly int Offset = offset;
    public readonly int Line = line;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEndOfSource()
    {
        return Offset >= Source.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadSymbol(ref readonly Symbol symbol)
    {
        return symbol.IsEndOfSource()
            ? "EOF"
            : symbol.IsError()
                ? $"error at line: {symbol.Line}, column: {symbol.Offset}"
                : Source[symbol.Offset..(symbol.Offset + symbol.Length)];
    }
}
