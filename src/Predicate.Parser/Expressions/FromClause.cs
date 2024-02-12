namespace Predicate.Parser.Expressions;

public sealed record FromClause(
    string Identifier,
    Expression WhereClause)
    : Expression
{
    public override void Print(string indent = "")
    {
        throw new NotImplementedException();
    }
}
