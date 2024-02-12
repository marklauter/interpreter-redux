namespace Predicate.Parser.Expressions;

public abstract record BinaryExpression(
    Expression Left,
    Expression Right)
    : Expression;
