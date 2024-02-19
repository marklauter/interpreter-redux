using Math.Parser.Exceptions;
using Math.Parser.Expressions;

namespace Math.Parser.Tests;

public sealed class MathParseTests(Parser parser)
{
    private readonly Parser parser = parser ?? throw new ArgumentNullException(nameof(parser));

    [Theory]
    [InlineData(" ")]
    [InlineData("x")]
    public void Parse_Throws_UnexpectedTokenException(string source)
    {
        var ex = Assert.Throws<UnexpectedTokenException>(() => parser.Parse(source));
        Assert.Contains(source, ex.Message);
    }

    [Theory]
    [InlineData("")]
    public void Parse_Throws_UnexpectedEndOfSourceException(string source)
    {
        var ex = Assert.Throws<UnexpectedEndOfSourceException>(() => parser.Parse(source));
        Assert.Contains(source, ex.Message);
    }

    [Theory]
    [InlineData("1", 1, NumericTypes.Integer)]
    [InlineData("12", 12, NumericTypes.Integer)]
    [InlineData("123", 123, NumericTypes.Integer)]
    [InlineData("123.4", 123.4, NumericTypes.FloatingPoint)]
    [InlineData("0.123", 0.123, NumericTypes.FloatingPoint)]
    [InlineData("1.234E-6", 1.234E-6, NumericTypes.ScientificNotation)]
    [InlineData("12.234E-6", 12.234E-6, NumericTypes.ScientificNotation)]
    public void Parse_Returns_Number(string source, double expectedValue, NumericTypes expectedType)
    {
        var expression = parser.Parse(source);
        var number = Assert.IsType<Number>(expression);
        Assert.Equal(expectedType, number.Type);
        switch (number.Type)
        {
            case NumericTypes.Integer:
                Assert.Equal(Convert.ToInt32(expectedValue), Convert.ToInt32(number.Evaluate()));
                break;
            case NumericTypes.FloatingPoint:
                Assert.Equal(expectedValue, number.Evaluate(), 4);
                break;
            case NumericTypes.NotANumber:
                Assert.Fail("NaN");
                break;
            case NumericTypes.ScientificNotation:
                Assert.Equal(expectedValue, number.Evaluate(), 15);
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
        var expression = parser.Parse(source);
        var root = Assert.IsType<BinaryOperation>(expression);
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
        var expression = parser.Parse(source);
        var root = Assert.IsType<BinaryOperation>(expression);
        Assert.Equal(Convert.ToInt32(expectedValue), Convert.ToInt32(root.Evaluate()));
    }

    [Theory]
    [InlineData("1 + 2 * 3", 7)]
    [InlineData("(1 + 2) * 3", 9)]
    public void Parse_Respects_Order_Of_Operations(string source, double expectedValue)
    {
        var expression = parser.Parse(source);
        var root = Assert.IsType<BinaryOperation>(expression);
        Assert.Equal(Convert.ToInt32(expectedValue), Convert.ToInt32(root.Evaluate()));
    }
}
