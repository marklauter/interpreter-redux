using Predicate.Parser.Exceptions;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Predicate.Parser.Expressions;

public enum LogicalOperators
{
    Error = 0,
    And = 1,
    Or = 2,
}

internal readonly ref struct LogicalOperator(LogicalOperators Operator)
{
    public LogicalOperators Operator { get; } = Operator;

    internal static string[] AsArray()
    {
        return [.. Operators.Keys];
    }

    private static readonly ReadOnlyDictionary<string, LogicalOperators> Operators =
        new Dictionary<string, LogicalOperators>
        {
            { "&&", LogicalOperators.And },
            { "||", LogicalOperators.Or }
        }.AsReadOnly();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ToSymbol(LogicalOperators logicalOperator)
    {
        return logicalOperator switch
        {
            LogicalOperators.Error => "error",
            LogicalOperators.And => "&&",
            LogicalOperators.Or => "||",
            _ => throw new ArgumentOutOfRangeException(nameof(logicalOperator)),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static LogicalOperators FromSymbol(string symbol)
    {
        return !Operators.TryGetValue(symbol, out var value)
            ? throw new ParseException($"Unknown logical operator: {symbol}")
            : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator LogicalOperator(string symbol)
    {
        var op = FromSymbol(symbol);
        return new LogicalOperator(op);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator string(LogicalOperator logicalOperator)
    {
        return ToSymbol(logicalOperator.Operator);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator LogicalOperators(LogicalOperator logicalOperator)
    {
        return logicalOperator.Operator;
    }
}

