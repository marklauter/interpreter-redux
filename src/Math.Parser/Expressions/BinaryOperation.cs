namespace Math.Parser.Expressions;

public sealed record BinaryOperation(
    Expression Left,
    Expression Right,
    int OperatorId)
    : Expression
{
    public override double Evaluate()
    {
        var left = Left.Evaluate();
        var right = Right.Evaluate();
        return OperatorId switch
        {
            TokenIds.ADD => left + right,
            TokenIds.SUBTRACT => left - right,
            TokenIds.MULTIPLY => left * right,
            TokenIds.DIVIDE => left / right,
            TokenIds.MODULUS => left % right,
            _ => throw new NotSupportedException($"unexpected operator {Operator}"),
        };
    }

    private string Operator => OperatorId switch
    {
        TokenIds.ADD => "+",
        TokenIds.SUBTRACT => "-",
        TokenIds.MULTIPLY => "*",
        TokenIds.DIVIDE => "/",
        TokenIds.MODULUS => "%",
        _ => throw new NotSupportedException($"unexpected operator {OperatorId}"),
    };

    public override void Print(string indent = "")
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Magenta;

        Console.WriteLine($"{indent}{nameof(BinaryOperation)}");
        Console.WriteLine($"{indent}Left");
        Left.Print($"{indent}   ");
        Console.WriteLine($"{indent}Op {Operator}");
        Console.WriteLine($"{indent}Right");
        Right.Print($"{indent}   ");

        Console.ForegroundColor = color;
    }

    public override IEnumerable<Expression> Children()
    {
        yield return Left;
        yield return Right;
    }
}
