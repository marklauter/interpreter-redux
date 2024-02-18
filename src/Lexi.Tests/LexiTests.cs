using System.Diagnostics.CodeAnalysis;

namespace Lexi.Tests;

[ExcludeFromCodeCoverage]
public sealed class LexiTests(Lexi lexer)
{
    public Lexi Lexer { get; } = lexer
        ?? throw new ArgumentNullException(nameof(lexer));

    [Theory]
    [InlineData("1", 0)]
    [InlineData("-1", 0)]
    [InlineData("10", 0)]
    [InlineData("-10", 0)]
    [InlineData("1.0", 1)]
    [InlineData("0.1", 1)]
    [InlineData("123.456", 1)]
    [InlineData("-123.456", 1)]
    [InlineData("+", 2)]
    [InlineData("-", 3)]
    [InlineData("*", 4)]
    [InlineData("/", 5)]
    [InlineData("%", 6)]
    [InlineData("<", 7)]
    [InlineData("<=", 8)]
    public void Test(string source, int expectedId)
    {
        var result = lexer.NextToken(new Script(source));
        Assert.Equal(expectedId, result.Symbol.TokenId);
        var symbol = result.Symbol;
        Assert.Equal(source, result.Script.ReadSymbol(in symbol));
    }

    [SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "unit test")]
    [Theory]
    [InlineData("1 -1 10 1.0 0.1 + -", new int[] { 0, 0, 0, 1, 1, 2, 3 })]
    public void ReadToEndOfSource(string source, int[] expectedId)
    {
        var symbols = source.Split(' ');
        var script = new Script(source);
        for (var i = 0; i < expectedId.Length; ++i)
        {
            var result = lexer.NextToken(script);
            Assert.Equal(expectedId[i], result.Symbol.TokenId);
            var symbol = result.Symbol;
            Assert.Equal(symbols[i], result.Script.ReadSymbol(in symbol));
            script = result.Script;
        }
    }
}
