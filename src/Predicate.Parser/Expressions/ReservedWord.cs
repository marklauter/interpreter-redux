using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Predicate.Parser.Expressions;

internal sealed record ReservedWord(
    ReservedWords Value)
    : Expression
{
    public ReservedWords Value { get; } = Value;

    internal static string[] AsArray()
    {
        return [.. ReservedWords.Keys];
    }

    private static readonly ReadOnlyDictionary<string, ReservedWords> ReservedWords =
        new Dictionary<string, ReservedWords>
        {
            { "from", Expressions.ReservedWords.From },
            { "where", Expressions.ReservedWords.Where },
            { "skip", Expressions.ReservedWords.Skip },
            { "take", Expressions.ReservedWords.Take },
        }.AsReadOnly();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ToSymbol(ReservedWords reservedWord)
    {
        return reservedWord switch
        {
            Expressions.ReservedWords.From => "from",
            Expressions.ReservedWords.Where => "where",
            Expressions.ReservedWords.Skip => "skip",
            Expressions.ReservedWords.Take => "take",
            Expressions.ReservedWords.Error or _ =>
                throw new ArgumentOutOfRangeException(nameof(reservedWord)),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ReservedWords FromSymbol(string symbol)
    {
        return !ReservedWords.TryGetValue(symbol, out var value)
            ? Expressions.ReservedWords.Error
            : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReservedWord(string symbol)
    {
        var reservedWord = FromSymbol(symbol);
        return new ReservedWord(reservedWord);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator string(ReservedWord reservedWord)
    {
        return ToSymbol(reservedWord.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReservedWords(ReservedWord reservedWord)
    {
        return reservedWord.Value;
    }
}
