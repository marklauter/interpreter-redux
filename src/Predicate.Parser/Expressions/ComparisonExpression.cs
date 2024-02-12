namespace Predicate.Parser.Expressions;

public sealed record ComparisonExpression(
    Expression Left,
    ComparisonOperators Operator,
    Expression Right)
    : BinaryExpression(Left, Right)
{
    public override void Print(string indent = "")
    {
        throw new NotImplementedException();
    }
}
