using System.Runtime.CompilerServices;

namespace Math.Parser;

public record Expression
{
    public virtual double Evaluate()
    {
        throw new InvalidOperationException("empty expression");
    }
};

public record BinaryOperation(
    Expression Left,
    Expression Right,
    Operators Operator)
    : Expression
{
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
}

public record Group(
    Expression Expression)
    : Expression
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override double Evaluate()
    {
        return Expression.Evaluate();
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
