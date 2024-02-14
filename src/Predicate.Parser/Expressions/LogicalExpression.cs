namespace Predicate.Parser.Expressions;

public sealed record LogicalExpression(
    Expression Left,
    LogicalOperators Operator,
    Expression Right)
    : BinaryExpression(Left, Right);
