using Predicate.Parser.Exceptions;
using Predicate.Parser.Expressions;

namespace Predicate.REPL;

internal static class StatementPrinter
{
    public static void Print(this Statement statement)
    {
        PrintExpressionType(nameof(statement.From));
        PrintExpressionValue((string)statement.From);
        Console.WriteLine();

        if (statement.Skip != null)
        {
            PrintExpressionType(nameof(statement.Skip));
            PrintExpressionValue((string)statement.Skip);
            Console.WriteLine();
        }

        if (statement.Take != null)
        {
            PrintExpressionType(nameof(statement.Take));
            PrintExpressionValue((string)statement.Take);
            Console.WriteLine();
        }

        PrintTree(statement.Predicate);
    }

    public static void Print(this ParseException exception)
    {
        var fgcolor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(exception.Message);
        if (exception.Data.Contains("errors"))
        {
            var errors = (IEnumerable<string>?)exception.Data["errors"];
            if (errors is not null)
            {
                foreach (var symbol in errors)
                {
                    Console.WriteLine(symbol);
                }
            }
        }

        Console.ForegroundColor = fgcolor;
    }

    public static void PrintTree(
        Expression expression,
        string indent = "")
    {
        if (expression is BinaryExpression binaryExpression)
        {
            PrintExpressionType(expression.GetType().Name, indent);
            Console.WriteLine();

            indent += "   ";
            PrintTree(binaryExpression.Left, indent);

            Console.WriteLine();
            PrintExpressionType("Operator", indent);
            PrintExpressionValue(ReadOperator(binaryExpression));
            Console.WriteLine();

            PrintTree(binaryExpression.Right, indent);
        }
        else
        {
            PrintExpression(expression, indent);
        }
    }

    private static void PrintExpressionType(string value, string indent = "")
    {
        var fgcolor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(indent);
        Console.Write(value);
        Console.ForegroundColor = fgcolor;
        Console.Write(": ");
    }

    private static void PrintExpressionValue(string value, string indent = "")
    {
        var fgcolor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(indent);
        Console.Write(value);
        Console.ForegroundColor = fgcolor;
    }

    private static string ReadOperator(BinaryExpression binaryExpression)
    {
        return binaryExpression switch
        {
            ComparisonExpression comparisonExpression => (ComparisonOperator)comparisonExpression.Operator,
            LogicalExpression logicalExpression => (LogicalOperator)logicalExpression.Operator,
            _ => "unexpected expression",
        };
    }

    private static void PrintExpression(
        Expression expression,
        string indent)
    {
        var value = expression switch
        {
            Identifier identifier => (string)identifier,
            ReservedWord reservedWord => (string)reservedWord,
            BooleanLiteral booleanLiteral => (string)booleanLiteral,
            StringLiteral stringLiteral => (string)stringLiteral,
            NumericLiteral numberLiteral => (string)numberLiteral,
            _ => "unexpected expression",
        };

        PrintExpressionType(expression.GetType().Name, indent);
        PrintExpressionValue(value);
    }
}
