using Predicate.Parser.Exceptions;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Predicate.Parser.Expressions;

public enum ComparisonOperators
{
    Error = 0,
    Equal = 1,
    NotEqual = 2,
    LessThan = 3,
    GreaterThan = 4,
    LessThanOrEqual = 5,
    GreaterThanOrEqual = 6,
    StartsWith = 7,
    EndsWith = 8,
    Contains = 9,
}

internal readonly ref struct ComparisonOperator(ComparisonOperators Operator)
{
    public ComparisonOperators Operator { get; } = Operator;

    internal static string[] AsArray()
    {
        return [.. Operators.Keys];
    }

    private static readonly ReadOnlyDictionary<string, ComparisonOperators> Operators =
        new Dictionary<string, ComparisonOperators>
        {
            { "==", ComparisonOperators.Equal },
            { "!=", ComparisonOperators.NotEqual },
            { ">",  ComparisonOperators.GreaterThan },
            { "<",  ComparisonOperators.LessThan },
            { ">=", ComparisonOperators.GreaterThanOrEqual },
            { "<=", ComparisonOperators.LessThanOrEqual },
            { "-s", ComparisonOperators.StartsWith },
            { "-e", ComparisonOperators.EndsWith },
            { "-c", ComparisonOperators.Contains }
        }.AsReadOnly();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ToSymbol(ComparisonOperators comparisonOperator)
    {
        return comparisonOperator switch
        {
            ComparisonOperators.Error => "error",
            ComparisonOperators.Equal => "==",
            ComparisonOperators.NotEqual => "!=",
            ComparisonOperators.LessThan => "<",
            ComparisonOperators.GreaterThan => ">",
            ComparisonOperators.LessThanOrEqual => "<=",
            ComparisonOperators.GreaterThanOrEqual => ">=",
            ComparisonOperators.StartsWith => "-s",
            ComparisonOperators.EndsWith => "-e",
            ComparisonOperators.Contains => "-c",
            _ => throw new ArgumentOutOfRangeException(nameof(comparisonOperator)),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ComparisonOperators FromSymbol(string symbol)
    {
        return !Operators.TryGetValue(symbol, out var value)
            ? throw new ParseException($"Unknown comparison operator: {symbol}")
            : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ComparisonOperator(string symbol)
    {
        var op = FromSymbol(symbol);
        return new ComparisonOperator(op);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator string(ComparisonOperator comparisonOperator)
    {
        return ToSymbol(comparisonOperator.Operator);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ComparisonOperators(ComparisonOperator comparisonOperator)
    {
        return comparisonOperator.Operator;
    }
}
