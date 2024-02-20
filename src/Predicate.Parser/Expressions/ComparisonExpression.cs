namespace Predicate.Parser.Expressions;

public sealed record ComparisonExpression(
    Expression Left,
    ComparisonOperators Operator,
    Expression Right)
    : BinaryExpression(Left, Right)
{
    public override string ToString()
    {
        return Operator switch
        {
            ComparisonOperators.Equal => nameof(ComparisonOperators.Equal),
            ComparisonOperators.NotEqual => nameof(ComparisonOperators.NotEqual),
            ComparisonOperators.GreaterThan => nameof(ComparisonOperators.GreaterThan),
            ComparisonOperators.GreaterThanOrEqual => nameof(ComparisonOperators.GreaterThanOrEqual),
            ComparisonOperators.LessThan => nameof(ComparisonOperators.LessThan),
            ComparisonOperators.LessThanOrEqual => nameof(ComparisonOperators.LessThanOrEqual),
            ComparisonOperators.StartsWith => nameof(ComparisonOperators.StartsWith),
            ComparisonOperators.EndsWith => nameof(ComparisonOperators.EndsWith),
            ComparisonOperators.Contains => nameof(ComparisonOperators.Contains),
            ComparisonOperators.Error or
            _ => throw new InvalidOperationException("Invalid comparison operator."),
        };
    }
}
