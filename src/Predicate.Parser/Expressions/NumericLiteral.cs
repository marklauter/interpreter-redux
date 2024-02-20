using System.Globalization;
using System.Runtime.CompilerServices;

namespace Predicate.Parser.Expressions;

public sealed record NumericLiteral(
    NumericTypes Type,
    double Value)
    : Expression
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNaN()
    {
        return Type == NumericTypes.NotANumber;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static NumericLiteral ParseNumericLiteral(string symbol)
    {
        if (!Double.TryParse(
            symbol,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out var value))
        {
            return new NumericLiteral(
                NumericTypes.NotANumber,
                Double.NaN);
        }

        var type = symbol.Contains('.')
            ? NumericTypes.FloatingPoint
            : NumericTypes.Integer;

        return new NumericLiteral(
            type,
            value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator int(NumericLiteral literal)
    {
        return Convert.ToInt32(Math.Round(literal.Value, 0));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NumericLiteral(int value)
    {
        return new(
            NumericTypes.Integer,
            Convert.ToDouble(value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator double(NumericLiteral literal)
    {
        return literal.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NumericLiteral(double value)
    {
        return new(
            NumericTypes.FloatingPoint,
            value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator string(NumericLiteral literal)
    {
        return literal.Type switch
        {
            NumericTypes.ScientificNotation or NumericTypes.FloatingPoint => literal.Value
                .ToString(CultureInfo.InvariantCulture),
            NumericTypes.Integer => Convert.ToInt32(Double.Round(literal.Value, 0))
                .ToString(CultureInfo.InvariantCulture),
            NumericTypes.NotANumber or _ => "NaN",
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NumericLiteral(string value)
    {
        return ParseNumericLiteral(value);
    }
}
