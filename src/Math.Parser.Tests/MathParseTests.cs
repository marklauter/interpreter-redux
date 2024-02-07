namespace Math.Parser.Tests;

public class MathParseTests(Parser parser)
{
    private readonly Parser parser = parser ?? throw new ArgumentNullException(nameof(parser));

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("x")]
    public void Parse_Returns_Empty_Expression(string source)
    {
        var ast = parser.Parse(source);
        var number = Assert.IsType<Number>(ast.Root);
        Assert.Equal(NumberTypes.NotANumber, number.Type);
    }

    [Theory]
    [InlineData("1", 1, NumberTypes.Integer)]
    [InlineData("12", 12, NumberTypes.Integer)]
    [InlineData("123", 123, NumberTypes.Integer)]
    [InlineData("123.4", 123.4, NumberTypes.Float)]
    [InlineData("0.123", 0.123, NumberTypes.Float)]
    public void Parse_Returns_Number(string source, double expectedValue, NumberTypes expectedType)
    {
        var ast = parser.Parse(source);
        var number = Assert.IsType<Number>(ast.Root);
        Assert.Equal(expectedType, number.Type);
        switch (number.Type)
        {
            case NumberTypes.Integer:
                Assert.Equal(Convert.ToInt32(expectedValue), Convert.ToInt32(number.Evaluate()));
                break;
            case NumberTypes.Float:
                Assert.Equal(expectedValue, number.Evaluate(), 4);
                break;
            case NumberTypes.NotANumber:
                Assert.Fail("NaN");
                break;
            default:
                Assert.Fail("default");
                break;
        }
    }

    [Theory]
    [InlineData("1 + 1", 2)]
    [InlineData("1 + 1 - 1", 1)]
    [InlineData("2 + 2 - 1", 3)]
    [InlineData("1 + 2 - 3", 0)]
    [InlineData("2 - 1 + 3", 4)]
    [InlineData("3 - 2 + 1", 2)]
    [InlineData("3 - 2 + 1 - 1", 1)]
    public void Add_Subtract_Evaluate_Returns_Expected_Value(string source, double expectedValue)
    {
        var ast = parser.Parse(source);
        var root = Assert.IsType<BinaryOperation>(ast.Root);
        Assert.Equal(Convert.ToInt32(expectedValue), Convert.ToInt32(root.Evaluate()));
    }

    [Theory]
    [InlineData("1 * 1", 1)]
    [InlineData("1 * 2", 2)]
    [InlineData("2 * 2 / 2", 2)]
    [InlineData("2 / 2 * 3", 3)]
    [InlineData("4 * 11 / 2", 22)]
    [InlineData("4 / 2 * 11", 22)]
    public void Multiply_Divide_Evaluate_Returns_Expected_Value(string source, double expectedValue)
    {
        var ast = parser.Parse(source);
        var root = Assert.IsType<BinaryOperation>(ast.Root);
        Assert.Equal(Convert.ToInt32(expectedValue), Convert.ToInt32(root.Evaluate()));
    }
}