namespace Math.Parser.Expressions;

public abstract record Expression
{
    public abstract double Evaluate();

    public abstract IEnumerable<Expression> Children();

    public abstract void Print(string indent = "");
};
