namespace Predicate.Parser.Expressions;

public sealed record LogicalExpression(
    Expression Left,
    LogicalOperators Operator,
    Expression Right)
    : BinaryExpression(Left, Right)
{
    public override void Print(string indent = "")
    {
        throw new NotImplementedException();
    }
}
