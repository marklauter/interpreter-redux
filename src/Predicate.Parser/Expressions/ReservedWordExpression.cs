namespace Predicate.Parser.Expressions;

public sealed record ReservedWordExpression(
    ReservedWords Value)
    : Expression
{
    public override void Print(string indent = "")
    {
        throw new NotImplementedException();
    }
}
