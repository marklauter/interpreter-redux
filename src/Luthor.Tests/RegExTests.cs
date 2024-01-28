using Luthor.Context;
using Luthor.Tokens;

namespace Luthor.Tests;

public sealed class RegExTests(LinguisticContext expressions)
{
    [Theory]
    [InlineData(@"""hello, world.""", "hello, world.", true)]
    [InlineData(@"""hello, \""world.\""""", @"hello, \""world.\""", true)]
    [InlineData(@"""hello, \""world.\"""" ""this is string two""", @"hello, \""world.\""", true)]
    [InlineData(@" ""hello, world.""", "", false)]
    public void StringLiterals(string value, string expected, bool expectedSuccess)
    {
        var regex = expressions[TokenType.StringLiteral].Regex;

        var match = regex.Match(value);
        Assert.Equal(expectedSuccess, match.Success);
        if (match.Success)
        {
            Assert.Equal(expected, match.Value[1..^1]);
        }
    }

    [Theory]
    [InlineData("x+y", "+", 1, true)]
    [InlineData("x+1", "+", 1, true)]
    [InlineData("x + y", "+", 2, true)]
    [InlineData("x + 1", "+", 2, true)]
    public void InfixOperators(string value, string expected, int position, bool expectedSuccess)
    {
        var expresion = expressions[TokenType.InfixOperator];

        var match = expresion.Regex.Match(value, position);
        Assert.Equal(expectedSuccess, match.Success);
        if (match.Success)
        {
            Assert.Equal(expected, match.Value);
        }
    }
}
