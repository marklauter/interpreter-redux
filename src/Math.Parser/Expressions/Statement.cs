namespace Math.Parser.Expressions;

public sealed record Statement(
    Expression Expression,
    string[] Errors)
{
    public double Evaluate()
    {
        return Expression.Evaluate();
    }

    public void Print()
    {
        Expression.Print();
    }
}
