using System.Data;

namespace Interpreter.Operators;

public class BooleanExpression
{
    public bool Evaluate()
    {
        throw new NotImplementedException();
    }
}

internal abstract class LogicalOperator
    : MetaOperator<bool, BooleanExpression>
{
    public override int OperandCount => 2;
}

internal class AndOperator
    : LogicalOperator
{
    public override string Symbol => "and";

    public override bool Evaluate(params BooleanExpression[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Length != OperandCount)
        {
            throw new InvalidExpressionException();
        }

        var leftOperand = args[0];
        var rightOperand = args[1];
        return leftOperand.Evaluate() && rightOperand.Evaluate();
    }
}

internal class OrOperator
    : LogicalOperator
{
    public override string Symbol => "or";

    public override bool Evaluate(params BooleanExpression[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Length != OperandCount)
        {
            throw new InvalidExpressionException();
        }

        var leftOperand = args[0];
        var rightOperand = args[1];
        return leftOperand.Evaluate() && rightOperand.Evaluate();
    }
}
