using Luthor.Context;
using Luthor.Spec;
using Luthor.Tokens;
using System.Text.RegularExpressions;

namespace Luthor.Tests;

public sealed class RegExTests(LanguageSpecification language)
{
    private readonly LanguageSpecification language = language
        ?? throw new ArgumentNullException(nameof(language));

    private readonly LinguisticContext context = new(language);

    [Theory]
    [InlineData(@"""hello, world.""", "hello, world.", true)]
    [InlineData(@"""hello, \""world.\""""", @"hello, \""world.\""", true)]
    [InlineData(@"""hello, \""world.\"""" ""this is string two""", @"hello, \""world.\""", true)]
    [InlineData(@" ""hello, world.""", "", false)]
    public void StringLiterals(string value, string expected, bool expectedSuccess)
    {
        var regex = new Regex(
            $@"\G(?:{LinguisticContext.StringLiteralExpression})",
            RegexOptions.Compiled);

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
        var expresion = context.LinguisticExpression(TokenType.InfixOperator);

        var match = expresion.Regex.Match(value, position);
        Assert.Equal(expectedSuccess, match.Success);
        if (match.Success)
        {
            Assert.Equal(expected, match.Value);
        }
    }
}
