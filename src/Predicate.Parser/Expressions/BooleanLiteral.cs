﻿using System.Runtime.CompilerServices;

namespace Predicate.Parser.Expressions;

public sealed record BooleanLiteral(
    bool Value)
    : Expression
{
    public override void Print(string indent = "")
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator bool(BooleanLiteral literal)
    {
        return literal.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator BooleanLiteral(bool value)
    {
        return new(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator string(BooleanLiteral literal)
    {
        return literal.Value.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator BooleanLiteral(string value)
    {
        return Boolean.TryParse(value, out var result)
            ? new(result)
            : throw new InvalidOperationException($"value is not a bool '{value}'");
    }
}
