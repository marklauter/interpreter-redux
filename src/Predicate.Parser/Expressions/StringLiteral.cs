using System.Runtime.CompilerServices;

namespace Predicate.Parser.Expressions;

public sealed record StringLiteral(
    string Value)
    : Expression
{
    public override void Print(string indent = "")
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator StringLiteral(string value)
    {
        var start = value.StartsWith('"')
            ? 1
            : 0;

        var end = value.EndsWith('"')
            ? value.Length - 1
            : value.Length;

        return new(value[start..end]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator string(StringLiteral literal)
    {
        return literal.Value;
    }
}
