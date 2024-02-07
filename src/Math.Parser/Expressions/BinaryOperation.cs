namespace Math.Parser.Expressions;

public record BinaryOperation(
    Expression Left,
    Expression Right,
    OperatorTypes Operator)
    : Expression
{
    public override double Evaluate()
    {
        var left = Left.Evaluate();
        var right = Right.Evaluate();
        return Operator switch
        {
            OperatorTypes.Add => left + right,
            OperatorTypes.Subtract => left - right,
            OperatorTypes.Multiply => left * right,
            OperatorTypes.Divide => left / right,
            OperatorTypes.Modulus => left % right,
            _ => throw new NotSupportedException($"unexpected operator {Operator}"),
        };
    }

    public override void Print(string indent = "")
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Magenta;

        Console.WriteLine($"{indent}{nameof(BinaryOperation)}");
        Console.WriteLine($"{indent}Left Expression");
        Left.Print($"{indent}   ");
        Console.WriteLine($"{indent}Op {Operator}");
        Console.WriteLine($"{indent}Right Expression");
        Right.Print($"{indent}   ");

        Console.ForegroundColor = color;
    }

    public override IEnumerable<Expression> Children()
    {
        yield return Left;
        yield return Right;
    }
}
