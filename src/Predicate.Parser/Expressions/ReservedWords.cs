using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Predicate.Parser.Expressions;

public enum ReservedWords
{
    Error = 0,
    From = 1,
    Where = 2,
    Skip = 3,
    Take = 4,
}

internal readonly ref struct ReservedWord(
    ReservedWords Value)
{
    public ReservedWords Value { get; } = Value;

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
            Expressions.ReservedWords.Error => "error",
            Expressions.ReservedWords.From => "from",
            Expressions.ReservedWords.Where => "where",
            _ => throw new ArgumentOutOfRangeException(nameof(reservedWord)),
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
