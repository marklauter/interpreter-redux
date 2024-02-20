using System.Runtime.CompilerServices;

namespace Predicate.Parser.Expressions;

public sealed record BooleanLiteral(
    bool Value)
    : Expression
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(BooleanLiteral literal)
    {
        return literal.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator BooleanLiteral(bool value)
    {
        return new(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator string(BooleanLiteral literal)
    {
        return literal.Value switch
        {
            true => "true",
            false => "false",
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator BooleanLiteral(string value)
    {
        return Boolean.TryParse(value, out var result)
            ? new(result)
            : throw new InvalidOperationException($"value is not a bool '{value}'");
    }
}
