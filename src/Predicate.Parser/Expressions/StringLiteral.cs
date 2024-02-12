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
        // todo: remove the quotes from the string
        return new(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator string(StringLiteral literal)
    {
        return literal.Value;
    }
}
