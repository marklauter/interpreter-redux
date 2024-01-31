using Luthor.Context;
using System.Text.RegularExpressions;

namespace Luthor.Tests;

public sealed class RegExTests(
    LexicalContext readers,
    LanguageSpecification languageSpecification)
{
    [Theory]
    [InlineData(@"""hello, world.""", "hello, world.", true)]
    [InlineData(@"""hello, \""world.\""""", @"hello, \""world.\""", true)]
    [InlineData(@"""hello, \""world.\"""" ""this is string two""", @"hello, \""world.\""", true)]
    [InlineData(@" ""hello, world.""", "", false)]
    public void StringLiterals(string source, string expectedSymbol, bool expectedSuccess)
    {
        var reader = readers[Tokens.StringLiteral];

        var result = reader.Invoke(source, 0, 0, 1);
        Assert.Equal(expectedSuccess, result.IsMatch);
        if (result.IsMatch)
        {
            Assert.Equal(expectedSymbol, result.Token.Symbol[1..^1]);
        }
    }

    private const RegexOptions ExpressionOptions =
        RegexOptions.CultureInvariant |
        RegexOptions.ExplicitCapture |
        RegexOptions.Compiled |
        RegexOptions.Singleline;

    //[Theory]
    //[InlineData("(}", false, 0, 0)]
    //[InlineData("(]", false, 0, 0)]
    //[InlineData("(/>", false, 0, 0)]
    //[InlineData("{)", false, 0, 0)]
    //[InlineData("{]", false, 0, 0)]
    //[InlineData("{/>", false, 0, 0)]
    //[InlineData("[}", false, 0, 0)]
    //[InlineData("[)", false, 0, 0)]
    //[InlineData("[/>", false, 0, 0)]
    //[InlineData("<}", false, 0, 0)]
    //[InlineData("<)", false, 0, 0)]
    //[InlineData("<]", false, 0, 0)]

    //[InlineData("()", true, 0, 1)]
    //[InlineData("( )", true, 0, 1)]
    //[InlineData("( 1 + 1 )", true, 0, 1)]
    //[InlineData("( x / y )", true, 0, 1)]
    //[InlineData("x / (y + z)", true, 4, 1)]
    //[InlineData("x / (y + (z / 3))", true, 4, 1)]
    //[InlineData("x / (y + z", false, 0, 1)]
    //[InlineData("x(1)", true, 1, 1)]
    //[InlineData("(", false, 0, 1)]
    //[InlineData(")", false, 0, 1)]

    //[InlineData("{}", true, 0, 1)]
    //[InlineData("{ }", true, 0, 1)]
    //[InlineData("{ x = 1 + 1; }", true, 0, 1)]
    //[InlineData("{ x / y }", true, 0, 1)]
    //[InlineData("x / {y + z}", true, 4, 1)]
    //[InlineData("x / {y + {z / 3}}", true, 4, 1)]
    //[InlineData("x / {y + z", false, 0, 1)]
    //[InlineData("x{1}", true, 1, 1)]
    //[InlineData("{", false, 0, 1)]
    //[InlineData("}", false, 0, 1)]

    //[InlineData("</>", true, 0, 1)]
    //[InlineData("< />", true, 0, 1)]
    //[InlineData("<tag/>", true, 0, 1)]
    //[InlineData("<tag />", true, 0, 1)]
    //[InlineData(@"<tag name=""mytag""/>", true, 0, 1)]
    //[InlineData(@"<tag name=""mytag"" />", true, 0, 1)]
    //[InlineData(@"<tag name=""mytag[]""/>", true, 0, 1)]
    //[InlineData(@"<tag name=""mytag[]"" />", true, 0, 1)]
    //[InlineData("<", false, 0, 1)]
    //[InlineData("<123>", false, 0, 1)]
    //[InlineData("< / >", false, 0, 1)]

    //[InlineData("[]", true, 0, 1)]
    //[InlineData("[ ]", true, 0, 1)]
    //[InlineData("x[1]", true, 1, 1)]
    //[InlineData("x[1] / x[2]", true, 1, 1)]
    //[InlineData("x[(x + y) *x] / x[2]", true, 1, 1)]
    //[InlineData("[", false, 0, 1)]
    //[InlineData("]", false, 0, 1)]
    //public void CircumfixDelimiters(string source, bool expectedSuccess, int expectedOffset, int expectedLength)
    //{
    //    Assert.True(languageSpecification
    //        .TryGetCircumfixDelimiterOpenPattern(out var pattern));
    //    Assert.NotNull(pattern);
    //    Assert.NotEmpty(pattern);

    //    var regex = new Regex($@"\G(?:{pattern})", ExpressionOptions);

    //    var match = regex.Match(source, expectedOffset);
    //    Assert.Equal(expectedSuccess, match.Success);
    //    if (match.Success)
    //    {
    //        Assert.Equal(expectedOffset, match.Index);
    //        //Assert.Equal(expectedLength, match.ValueSpan.Length);
    //        Assert.Equal(expectedLength, expectedLength);
    //    }
    //}
}
