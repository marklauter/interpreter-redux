using System.Runtime.CompilerServices;

namespace Math.Parser.Expressions;

public record Number(
    NumberTypes Type,
    double Value)
    : Expression
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNaN()
    {
        return Type == NumberTypes.NotANumber;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override double Evaluate()
    {
        return Value;
    }

    public override IEnumerable<Expression> Children()
    {
        return Enumerable.Empty<Expression>();
    }

    public override void Print(string indent = "")
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        var value = IsNaN()
            ? "NaN"
            : $"{Value}";

        Console.WriteLine($"{indent}{nameof(Number)}: {value}");
        Console.ForegroundColor = color;
    }
}
