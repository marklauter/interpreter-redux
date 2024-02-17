using System.Diagnostics.CodeAnalysis;

namespace Luthor.Tests;

[ExcludeFromCodeCoverage]
public sealed class LexiTests(TokenDefinition[] tokenDefs)
{
    private readonly TokenDefinition[] tokenDefs = tokenDefs
        ?? throw new ArgumentNullException(nameof(tokenDefs));

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
    [InlineData("-", 2)]
    [InlineData("*", 3)]
    [InlineData("/", 3)]
    [InlineData("%", 3)]
    public void Test(string source, int expectedCode)
    {
        var lexi = new Lexi(source, tokenDefs);
        var token = lexi.NextToken();
        Assert.Equal(expectedCode, token.Code);
        Assert.Equal(source, lexi.ReadSymbol(in token));
    }
}
