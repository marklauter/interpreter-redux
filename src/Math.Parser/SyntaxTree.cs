namespace Math.Parser;

public sealed record SyntaxTree(
    Expression Root,
    string[] Errors)
{
    public double Evaluate()
    {
        return Root.Evaluate();
    }
}
