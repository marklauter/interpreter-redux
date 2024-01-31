using Luthor.Context;

namespace Luthor.Tests;

public sealed class RegExTests(LexicalContext context)
{
    private readonly LexicalContext context = context ?? throw new ArgumentNullException(nameof(context));

    [Theory]
    [InlineData(@"""hello, world.""", "hello, world.", true)]
    [InlineData(@"""hello, \""world.\""""", @"hello, \""world.\""", true)]
    [InlineData(@"""hello, \""world.\"""" ""this is string two""", @"hello, \""world.\""", true)]
    [InlineData(@" ""hello, world.""", "", false)]
    public void StringLiterals(string source, string expectedSymbol, bool expectedSuccess)
    {
        var matcher = context[Tokens.StringLiteral];

        var match = default(MatchResult);
        matcher(source, 0, 0, 1, ref match);

        Assert.Equal(expectedSuccess, match.IsMatch());
        if (match.IsMatch())
        {
            Assert.Equal(expectedSymbol, match.Token.Symbol[1..^1]);
        }
    }

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
    //public void CircumfixDelimiters(string source, string expectedSymbol, bool expectedSuccess)
    //{
    //    var matcher = context[Tokens.OpenCircumfixDelimiter];

    //    var match = matcher(source, 0, 0, 1);
    //    Assert.Equal(expectedSuccess, match.IsMatch());
    //    if (match.IsMatch())
    //    {
    //        Assert.Equal(expectedSymbol, match.Token.Symbol[1..^1]);
    //    }
    //}
}
