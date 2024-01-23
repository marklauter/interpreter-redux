using Interpreter.SyntacticAnalysis;

namespace Interpreter.Tests;

public sealed class ParserTests(Parser parser)
{
    private readonly Parser parser = parser ?? throw new ArgumentNullException(nameof(parser));

    [Theory]
    [InlineData("jelloBaby", typeof(IdentifierExpression))]
    [InlineData("42.42", typeof(DecimalConstantExpression))]
    [InlineData("42", typeof(IntegerConstantExpression))]
    [InlineData("\"42\"", typeof(StringConstantExpression))]
    public void Test(string source, Type expectedType)
    {
        var block = parser.Parse(source);

        Assert.NotEmpty(block.Expressions);
        Assert.Equal(expectedType, block.Expressions.First().GetType());
    }
}
