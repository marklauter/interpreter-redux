using System.Runtime.CompilerServices;

namespace Predicate.Parser.Expressions;

public sealed record ComparisonOperator(
    ComparisonOperators Value)
    : Expression
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ComparisonOperator(int tokenId)
    {
        return tokenId switch
        {
            TokenIds.EQUAL => new(ComparisonOperators.Equal),
            TokenIds.NOT_EQUAL => new(ComparisonOperators.NotEqual),
            TokenIds.LESS_THAN => new(ComparisonOperators.LessThan),
            TokenIds.GREATER_THAN => new(ComparisonOperators.GreaterThan),
            TokenIds.LESS_THAN_OR_EQUAL => new(ComparisonOperators.LessThanOrEqual),
            TokenIds.GREATER_THAN_OR_EQUAL => new(ComparisonOperators.GreaterThanOrEqual),
            TokenIds.STARTS_WITH => new(ComparisonOperators.StartsWith),
            TokenIds.ENDS_WITH => new(ComparisonOperators.EndsWith),
            TokenIds.CONTAINS => new(ComparisonOperators.Contains),
            _ => throw new ArgumentOutOfRangeException(nameof(tokenId)),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ComparisonOperators(ComparisonOperator value)
    {
        return value.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ComparisonOperator(ComparisonOperators value)
    {
        return new ComparisonOperator(value);
    }
}
