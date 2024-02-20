using Predicate.Parser.Exceptions;
using System.Runtime.CompilerServices;

namespace Predicate.Parser.Expressions;

public sealed record Keyword(
    Keywords Value)
    : Expression
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Keyword(int tokenId)
    {
        return tokenId switch
        {
            TokenIds.FROM => new Keyword(Keywords.From),
            TokenIds.WHERE => new Keyword(Keywords.Where),
            TokenIds.SKIP => new Keyword(Keywords.Skip),
            TokenIds.TAKE => new Keyword(Keywords.Take),
            _ => throw new ArgumentOutOfRangeException(nameof(tokenId)),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Keywords(Keyword value)
    {
        return value.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Keyword(Keywords value)
    {
        return new Keyword(value);
    }

    public override string ToString()
    {
        return Value switch
        {
            Keywords.From => "from",
            Keywords.Where => "where",
            Keywords.Skip => "skip",
            Keywords.Take => "take",
            Keywords.Error => throw new ParseException($"{nameof(Keywords)}.{nameof(Keywords.Error)}"),
            _ => throw new ArgumentOutOfRangeException(nameof(Value)),
        };
    }
}
