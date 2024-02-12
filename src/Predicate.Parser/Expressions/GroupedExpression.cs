namespace Predicate.Parser.Expressions;

public sealed record GroupedExpression(
    Expression Expression)
    : Expression
{
    public override void Print(string indent = "")
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;

        Console.WriteLine($"{indent}{nameof(GroupedExpression)} Expression");
        Expression.Print($"{indent}   ");

        Console.ForegroundColor = color;
    }
}
