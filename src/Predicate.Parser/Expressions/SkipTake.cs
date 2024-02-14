using System.Globalization;
using System.Runtime.CompilerServices;

namespace Predicate.Parser.Expressions;

public sealed record class Skip(
    int Value)
    : Expression
{
    public override void Print(string indent = "")
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator string(Skip value)
    {
        return value.Value.ToString(CultureInfo.InvariantCulture);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Skip(string value)
    {
        return Int32.TryParse(value, out var result)
            ? new(result)
            : throw new InvalidOperationException($"'{value}' is not an integer");
    }

}

public sealed record class Take(
    int Value)
    : Expression
{
    public override void Print(string indent = "")
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator string(Take value)
    {
        return value.Value.ToString(CultureInfo.InvariantCulture);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Take(string value)
    {
        return Int32.TryParse(value, out var result)
            ? new(result)
            : throw new InvalidOperationException($"'{value}' is not an integer");
    }
}
