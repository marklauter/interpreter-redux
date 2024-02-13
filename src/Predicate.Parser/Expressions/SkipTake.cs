namespace Predicate.Parser.Expressions;

public sealed record class SkipTake(
    int Skip,
    int Take)
    : Expression
{
    public override void Print(string indent = "")
    {
        throw new NotImplementedException();
    }
}
