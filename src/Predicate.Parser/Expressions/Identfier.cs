using System.Runtime.CompilerServices;

namespace Predicate.Parser.Expressions;

public sealed record Identfier(
    string Value)
    : Expression
{
    public override void Print(string indent = "")
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Identfier(string value)
    {
        return new(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator string(Identfier identifier)
    {
        return identifier.Value;
    }
}
