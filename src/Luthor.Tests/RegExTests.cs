using Luthor.Tokens;
using System.Diagnostics.CodeAnalysis;

namespace Luthor.Tests;

[ExcludeFromCodeCoverage]
public sealed class RegExTests(Tokenizers tokenizers)
{
    private readonly Tokenizers tokenizers = tokenizers ?? throw new ArgumentNullException(nameof(tokenizers));

    [Theory]
    [InlineData(@"""hello, world.""", "hello, world.", true)]
    [InlineData(@"""hello, \""world.\""""", @"hello, \""world.\""", true)]
    [InlineData(@"""hello, \""world.\"""" ""this is string two""", @"hello, \""world.\""", true)]
    [InlineData(@" ""hello, world.""", "", false)]
    public void StringLiterals(string source, string expectedSymbol, bool expectedSuccess)
    {
        var matcher = tokenizers[TokenType.StringLiteral];

        var token = matcher(source, 0);

        Assert.Equal(expectedSuccess, token.IsMatch());
        if (token.IsMatch())
        {
            Assert.Equal(expectedSymbol, Lexer.ReadSymbol(source, token)[1..^1]);
        }
    }
}
