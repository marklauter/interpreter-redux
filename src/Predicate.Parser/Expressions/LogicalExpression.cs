namespace Predicate.Parser.Expressions;

public sealed record LogicalExpression(
    Expression Left,
    LogicalOperators Operator,
    Expression Right)
    : BinaryExpression(Left, Right)
{
    public override string ToString()
    {
        return Operator switch
        {
            LogicalOperators.And => nameof(LogicalOperators.And),
            LogicalOperators.Or => nameof(LogicalOperators.Or),
            LogicalOperators.Error or
            _ => throw new InvalidOperationException("Invalid logical operator."),
        };
    }
}
