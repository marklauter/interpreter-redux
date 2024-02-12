using System.Runtime.CompilerServices;

namespace Math.Parser.Expressions;

public sealed record Group(
    Expression Expression)
    : Expression
{
    public override IEnumerable<Expression> Children()
    {
        yield return Expression;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override double Evaluate()
    {
        return Expression.Evaluate();
    }

    public override void Print(string indent = "")
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;

        Console.WriteLine($"{indent}{nameof(Group)} Expression");
        Expression.Print($"{indent}   ");

        Console.ForegroundColor = color;
    }
}
