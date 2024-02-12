namespace Predicate.Parser.Expressions;

public abstract record Expression
{
    public abstract void Print(string indent = "");
}
