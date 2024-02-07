using System.Runtime.CompilerServices;

namespace Math.Parser;

public abstract record Expression
{
    public abstract double Evaluate();

    public abstract IEnumerable<Expression> Children();

    public abstract void Print(string indent = "");
};

public record BinaryOperation(
    Expression Left,
    Expression Right,
    Operators Operator)
    : Expression
{
    public override void Print(string indent = "")
    {
        Console.WriteLine($"{indent}{nameof(BinaryOperation)}");
        Console.WriteLine($"{indent}Left");
        Left.Print($"{indent}    ");
        Console.WriteLine($"{indent}{Operator}");
        Console.WriteLine($"{indent}Right");
        Right.Print($"{indent}    ");
    }

    public override IEnumerable<Expression> Children()
    {
        yield return Left;
        yield return Right;
    }

    public override double Evaluate()
    {
        var left = Left.Evaluate();
        var right = Right.Evaluate();
        return Operator switch
        {
            Operators.Add => left + right,
            Operators.Subtract => left - right,
            Operators.Multiply => left * right,
            Operators.Divide => left / right,
            Operators.Modulus => left % right,
            _ => throw new NotSupportedException($"unexpected operator {Operator}"),
        };
    }
}

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
        Console.WriteLine($"{indent}{nameof(Number)}: {Value}");
    }
}

public record Group(
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
        Console.WriteLine($"{indent}{nameof(Group)}");
        Console.WriteLine($"{indent}Expression");
        Expression.Print($"{indent}    ");
    }
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "<Pending>")]
public enum NumberTypes
{
    NotANumber = 0, // NaN
    Integer = 1,
    Float = 2,
}

public enum Operators
{
    Add = 0,
    Subtract = 1,
    Multiply = 2,
    Divide = 3,
    Modulus = 4,
}
