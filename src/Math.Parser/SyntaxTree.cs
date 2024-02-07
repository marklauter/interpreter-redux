namespace Math.Parser;

public sealed record SyntaxTree(
    Expression Root,
    string[] Errors)
{
    public double Evaluate()
    {
        return Root.Evaluate();
    }

    public void Print()
    {
        Root.Print();
    }
}
