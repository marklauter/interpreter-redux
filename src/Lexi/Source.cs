using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Lexi;

[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "it's a struct")]
public readonly ref struct Source(
    string text,
    int offset)
{
    public Source(string source)
        : this(source, 0)
    {
    }

    public readonly string Text = text ?? throw new ArgumentNullException(nameof(text));
    public readonly int Offset = offset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEndOfSource()
    {
        return Offset >= Text.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadSymbol(ref readonly Symbol symbol)
    {
        return symbol.IsEndOfSource()
            ? "EOF"
            : symbol.IsError()
                ? $"lexer error at offset: {symbol.Offset}"
                : Text[symbol.Offset..(symbol.Offset + symbol.Length)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator string(Source script)
    {
        return script.Text;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Source(string source)
    {
        return new(source);
    }
}
