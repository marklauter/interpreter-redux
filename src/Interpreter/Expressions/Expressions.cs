namespace Interpreter.Expressions;

public record Expression<T>
{
    public virtual T? Evaluate()
    {
        return default;
    }
}

public record BooleanExpression
    : Expression<bool>
{
    public override bool Evaluate()
    {
        return true;
    }
}

public enum RelationalOperator
{
    Equal,
    NotEqual,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
}

public record ConditionalExpression(
    string PropertyName,
    object Value,
    RelationalOperator Operator)
    : BooleanExpression;

public enum LogicalOperator
{
    And,
    Or,
    Nor,
    Xor,
    Nand
}

public record LogicalExpression(
    BooleanExpression LeftOperand,
    BooleanExpression RightOperand,
    LogicalOperator Operator)
    : BooleanExpression;
