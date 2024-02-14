namespace Predicate.Parser.Expressions;

public sealed record ParentheticalExpression(
    Expression Expression)
    : Expression;
