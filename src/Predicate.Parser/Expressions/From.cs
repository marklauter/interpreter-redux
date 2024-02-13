namespace Predicate.Parser.Expressions;

public sealed record From(
    string Identifier,
    Expression Where)
    : Expression
{
    public override void Print(string indent = "")
    {
        throw new NotImplementedException();
    }
}
