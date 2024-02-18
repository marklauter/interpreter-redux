using System.Runtime.CompilerServices;

namespace Lexi;

public sealed record Script(
    string Source,
    int Offset,
    int Line)
{
    public Script(string source)
        : this(source, 0, 0)
    {
    }

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
