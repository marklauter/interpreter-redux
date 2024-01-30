using Luthor.Context;
using Luthor.Tokens;
using System.Text.RegularExpressions;

namespace Luthor.Tests;

public sealed class RegExTests(
    LinguisticContext expressions,
    LanguageSpecification languageSpecification)
{
    [Theory]
    [InlineData(@"""hello, world.""", "hello, world.", true)]
    [InlineData(@"""hello, \""world.\""""", @"hello, \""world.\""", true)]
    [InlineData(@"""hello, \""world.\"""" ""this is string two""", @"hello, \""world.\""", true)]
    [InlineData(@" ""hello, world.""", "", false)]
    public void StringLiterals(string value, string expected, bool expectedSuccess)
    {
        var regex = expressions[TokenType.StringLiteral].Expression;

        var match = regex.Match(value);
        Assert.Equal(expectedSuccess, match.Success);
        if (match.Success)
        {
            Assert.Equal(expected, match.Value[1..^1]);
        }
    }

    [Theory]
    [InlineData("(}", false, 0, 0)]
    [InlineData("(]", false, 0, 0)]
    [InlineData("(/>", false, 0, 0)]
    [InlineData("{)", false, 0, 0)]
    [InlineData("{]", false, 0, 0)]
    [InlineData("{/>", false, 0, 0)]
    [InlineData("[}", false, 0, 0)]
    [InlineData("[)", false, 0, 0)]
    [InlineData("[/>", false, 0, 0)]
    [InlineData("<}", false, 0, 0)]
    [InlineData("<)", false, 0, 0)]
    [InlineData("<]", false, 0, 0)]

    [InlineData("()", true, 0, 1)]
    [InlineData("( )", true, 0, 1)]
    [InlineData("( 1 + 1 )", true, 0, 1)]
    [InlineData("( x / y )", true, 0, 1)]
    [InlineData("x / (y + z)", true, 4, 1)]
    [InlineData("x / (y + (z / 3))", true, 4, 1)]
    [InlineData("x / (y + z", false, 0, 1)]
    [InlineData("x(1)", true, 1, 1)]
    [InlineData("(", false, 0, 1)]
    [InlineData(")", false, 0, 1)]

    [InlineData("{}", true, 0, 1)]
    [InlineData("{ }", true, 0, 1)]
    [InlineData("{ x = 1 + 1; }", true, 0, 1)]
    [InlineData("{ x / y }", true, 0, 1)]
    [InlineData("x / {y + z}", true, 4, 1)]
    [InlineData("x / {y + {z / 3}}", true, 4, 1)]
    [InlineData("x / {y + z", false, 0, 1)]
    [InlineData("x{1}", true, 1, 1)]
    [InlineData("{", false, 0, 1)]
    [InlineData("}", false, 0, 1)]

    [InlineData("</>", true, 0, 1)]
    [InlineData("< />", true, 0, 1)]
    [InlineData("<tag/>", true, 0, 1)]
    [InlineData("<tag />", true, 0, 1)]
    [InlineData(@"<tag name=""mytag""/>", true, 0, 1)]
    [InlineData(@"<tag name=""mytag"" />", true, 0, 1)]
    [InlineData(@"<tag name=""mytag[]""/>", true, 0, 1)]
    [InlineData(@"<tag name=""mytag[]"" />", true, 0, 1)]
    [InlineData("<", false, 0, 1)]
    [InlineData("<123>", false, 0, 1)]
    [InlineData("< / >", false, 0, 1)]

    [InlineData("[]", true, 0, 1)]
    [InlineData("[ ]", true, 0, 1)]
    [InlineData("x[1]", true, 1, 1)]
    [InlineData("x[1] / x[2]", true, 1, 1)]
    [InlineData("x[(x + y) *x] / x[2]", true, 1, 1)]
    [InlineData("[", false, 0, 1)]
    [InlineData("]", false, 0, 1)]
    public void CircumfixDelimiters(string source, bool expectedSuccess, int expectedOffset, int expectedLength)
    {
        Assert.True(languageSpecification
            .TryGetCircumfixDelimiterOpenPattern(out var pattern));
        Assert.NotNull(pattern);
        Assert.NotEmpty(pattern);

        var regex = new Regex(pattern);

        var match = regex.Match(source);
        Assert.Equal(expectedSuccess, match.Success);
        if (match.Success)
        {
            Assert.Equal(expectedOffset, match.Index);
            //Assert.Equal(expectedLength, match.ValueSpan.Length);
            Assert.Equal(expectedLength, expectedLength);
        }
    }

    [Fact]
    public void SquareBracket()
    {
        // \((?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!))\) 

        var char1 = "[";
        var char2 = "]";
        var pattern = $@"[{Regex.Escape(char1)}\{char2}]";
        var source = "123[567]";

        var match = Regex.Match(source, pattern);
        Assert.True(match.Success);
    }
}
