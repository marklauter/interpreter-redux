using System.Runtime.CompilerServices;

namespace Predicate.Parser.Expressions;

public sealed record CharacterLiteral(
    string Value)
    : Expression
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CharacterLiteral(string value)
    {
        var start = value.StartsWith('\'')
            ? 1
            : 0;

        var end = value.EndsWith('\'')
            ? value.Length - 1
            : value.Length;

        return new(value[start..end]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator string(CharacterLiteral literal)
    {
        return literal.Value;
    }
}
