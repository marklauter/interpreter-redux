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
        var expression = parser.Parse(source);
        _ = Assert.IsType<Expression>(expression);
    }

    [Theory]
    [InlineData("1", 1, NumberTypes.Integer)]
    [InlineData("12", 12, NumberTypes.Integer)]
    [InlineData("123", 123, NumberTypes.Integer)]
    [InlineData("123.4", 123.4, NumberTypes.Float)]
    [InlineData("0.123", 0.123, NumberTypes.Float)]
    public void Parse_Returns_Number(string source, double expectedValue, NumberTypes expectedType)
    {
        var expression = parser.Parse(source);
        var number = Assert.IsType<Number>(expression);
        Assert.Equal(expectedType, number.Type);
        switch (number.Type)
        {
            case NumberTypes.Integer:
                Assert.Equal(Convert.ToInt32(expectedValue), Convert.ToInt32(number.Value));
                break;
            case NumberTypes.Float:
                Assert.Equal(expectedValue, number.Value, 4);
                break;
        }
    }
}
