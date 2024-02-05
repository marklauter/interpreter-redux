using Luthor.Tokens;
using System.Diagnostics.CodeAnalysis;

namespace Luthor.Tests;

[ExcludeFromCodeCoverage]
public sealed class LexerTests(Tokenizers tokenizers)
{
    private readonly Tokenizers tokenizers = tokenizers ?? throw new ArgumentNullException(nameof(tokenizers));

    [Theory]
    [InlineData("2a", @"{""line"": 1, ""column"": 0}", 0, 0, TokenType.Error)]

    [InlineData(" ", "", 0, 1, TokenType.Whitespace)]
    [InlineData("  ", "", 0, 2, TokenType.Whitespace)]
    [InlineData(" x", "", 0, 1, TokenType.Whitespace)]

    [InlineData("", "", 0, 0, TokenType.EndOfSource)]

    [InlineData("\r", "", 0, 1, TokenType.NewLine)]
    [InlineData("\n", "", 0, 1, TokenType.NewLine)]
    [InlineData("\r\n", "", 0, 2, TokenType.NewLine)]

    [InlineData("if", "if", 0, 2, TokenType.ReservedWord)]
    [InlineData("else", "else", 0, 4, TokenType.ReservedWord)]
    [InlineData("let", "let", 0, 3, TokenType.ReservedWord)]

    [InlineData("x", "x", 0, 1, TokenType.Identifier)]
    [InlineData("xy", "xy", 0, 2, TokenType.Identifier)]
    [InlineData("iffy", "iffy", 0, 4, TokenType.Identifier)]
    [InlineData("letMe", "letMe", 0, 5, TokenType.Identifier)]
    [InlineData("miffed", "miffed", 0, 6, TokenType.Identifier)]
    [InlineData("bif", "bif", 0, 3, TokenType.Identifier)]

    [InlineData("1", "1", 0, 1, TokenType.NumericLiteral)]
    [InlineData("12", "12", 0, 2, TokenType.NumericLiteral)]
    [InlineData("123", "123", 0, 3, TokenType.NumericLiteral)]
    [InlineData("1234", "1234", 0, 4, TokenType.NumericLiteral)]
    [InlineData("1234.0", "1234.0", 0, 6, TokenType.NumericLiteral)]
    [InlineData("123.40", "123.40", 0, 6, TokenType.NumericLiteral)]
    [InlineData("12.340", "12.340", 0, 6, TokenType.NumericLiteral)]
    [InlineData("1.2340", "1.2340", 0, 6, TokenType.NumericLiteral)]
    [InlineData("0.1234", "0.1234", 0, 6, TokenType.NumericLiteral)]
    [InlineData("1234. ", @"{""line"": 1, ""column"": 0}", 0, 0, TokenType.Error)]

    [InlineData(@"""hello""", @"""hello""", 0, 7, TokenType.StringLiteral)]
    [InlineData(@"""hello \""world\""""", @"""hello \""world\""""", 0, 17, TokenType.StringLiteral)]
    [InlineData(@"""hello", @"{""line"": 1, ""column"": 0}", 0, 0, TokenType.Error)]

    [InlineData("true", "true", 0, 4, TokenType.BooleanLiteral)]
    [InlineData("false", "false", 0, 5, TokenType.BooleanLiteral)]

    [InlineData("'a'", "'a'", 0, 3, TokenType.CharacterLiteral)]
    [InlineData("'a '", @"{""line"": 1, ""column"": 0}", 0, 0, TokenType.Error)]

    [InlineData("// comment", "// comment", 0, 10, TokenType.Comment)]
    [InlineData("# comment", "# comment", 0, 9, TokenType.Comment)]

    [InlineData(";", ";", 0, 1, TokenType.InfixDelimiter)]
    [InlineData(",", ",", 0, 1, TokenType.InfixDelimiter)]

    [InlineData("x+y", "+", 1, 1, TokenType.Operator)]
    [InlineData("x + y", "+", 2, 1, TokenType.Operator)]
    [InlineData("x<=y", "<=", 1, 2, TokenType.Operator)]
    [InlineData("x <= y", "<=", 2, 2, TokenType.Operator)]
    [InlineData("i<1", "<", 1, 1, TokenType.Operator)]

    [InlineData("!i", "!", 0, 1, TokenType.Operator)]
    [InlineData("! i", "!", 0, 1, TokenType.Operator)]
    [InlineData("++i", "++", 0, 2, TokenType.Operator)]
    [InlineData("++ i", "++", 0, 2, TokenType.Operator)]
    [InlineData("i++", "++", 1, 2, TokenType.Operator)]
    [InlineData("i ++", "++", 2, 2, TokenType.Operator)]
    public void Returns_Expected_Token(
        string source,
        string expectedSymbol,
        int expectedOffset,
        int expectedLength,
        TokenType expectedToken)
    {
        var lexer = new Lexer(tokenizers, source);
        var token = lexer.NextToken();
        if (expectedOffset > 0)
        {
            while (token.Offset < expectedOffset && token.Type != TokenType.EndOfSource)
            {
                token = lexer.NextToken();
                if (token.IsError())
                {
                    Assert.Fail(token.Symbol);
                }
            }
        }

        Assert.True(
            expectedToken == TokenType.Error && !token.IsMatch()
            || expectedToken != TokenType.Error && token.IsMatch());

        Assert.Equal(expectedToken, token.Type);
        Assert.Equal(expectedLength, token.Length);
        Assert.Equal(expectedSymbol, token.Symbol);
        Assert.Equal(expectedOffset, token.Offset);
    }

    [Theory]
    [InlineData("if", 0, 2, TokenType.ReservedWord)]
    [InlineData("", 2, 1, TokenType.Whitespace)]
    [InlineData("(", 3, 1, TokenType.OpenCircumfixDelimiter)]
    [InlineData("a", 4, 1, TokenType.Identifier)]
    [InlineData("", 5, 1, TokenType.Whitespace)]
    [InlineData("+", 6, 1, TokenType.Operator)]
    [InlineData("", 7, 1, TokenType.Whitespace)]
    [InlineData("b", 8, 1, TokenType.Identifier)]
    [InlineData(")", 9, 1, TokenType.CloseCircumfixDelimiter)]
    [InlineData("", 10, 1, TokenType.Whitespace)]
    [InlineData("{", 11, 1, TokenType.OpenCircumfixDelimiter)]
    [InlineData("", 12, 1, TokenType.Whitespace)]
    [InlineData("let", 13, 3, TokenType.ReservedWord)]
    [InlineData("", 16, 1, TokenType.Whitespace)]
    [InlineData("c", 17, 1, TokenType.Identifier)]
    [InlineData("", 18, 1, TokenType.Whitespace)]
    [InlineData("=", 19, 1, TokenType.Operator)]
    [InlineData("", 20, 1, TokenType.Whitespace)]
    [InlineData("a", 21, 1, TokenType.Identifier)]
    [InlineData("", 22, 1, TokenType.Whitespace)]
    [InlineData("+", 23, 1, TokenType.Operator)]
    [InlineData("", 24, 1, TokenType.Whitespace)]
    [InlineData("b", 25, 1, TokenType.Identifier)]
    [InlineData("^", 26, 1, TokenType.Operator)]
    [InlineData("2", 27, 1, TokenType.NumericLiteral)]
    [InlineData(";", 28, 1, TokenType.InfixDelimiter)]
    [InlineData("", 29, 1, TokenType.Whitespace)]
    [InlineData("}", 30, 1, TokenType.CloseCircumfixDelimiter)]
    [InlineData("", 31, 0, TokenType.EndOfSource)]
    public void Returns_Expected_Token_From_MultiToken_Source(
        string expectedSymbol,
        int expectedOffset,
        int expectedLength,
        TokenType expectedToken)
    {
        var source = "if (a + b) { let c = a + b^2; }";
        var lexer = new Lexer(tokenizers, source);
        var token = lexer.NextToken();
        if (expectedOffset > 0)
        {
            while (token.Offset < expectedOffset && token.Type != TokenType.EndOfSource)
            {
                token = lexer.NextToken();
                if (token.IsError())
                {
                    Assert.Fail(token.Symbol);
                }
            }
        }

        Assert.True(
            expectedToken == TokenType.Error && !token.IsMatch()
            || expectedToken != TokenType.Error && token.IsMatch());

        Assert.Equal(expectedToken, token.Type);
        Assert.Equal(expectedLength, token.Length);
        Assert.Equal(expectedSymbol, token.Symbol);
        Assert.Equal(expectedOffset, token.Offset);
    }

    [Theory]
    [InlineData("1.0", 0, 3, TokenType.NumericLiteral)]
    [InlineData("", 3, 1, TokenType.Whitespace)]
    [InlineData("2.0", 4, 3, TokenType.NumericLiteral)]
    [InlineData("", 7, 1, TokenType.Whitespace)]
    [InlineData("3.0", 8, 3, TokenType.NumericLiteral)]
    [InlineData("", 11, 1, TokenType.Whitespace)]
    [InlineData("397.173", 12, 7, TokenType.NumericLiteral)]
    [InlineData("", 19, 1, TokenType.Whitespace)]
    [InlineData("89", 20, 2, TokenType.NumericLiteral)]
    [InlineData("", 22, 1, TokenType.Whitespace)]
    [InlineData("0.1", 23, 3, TokenType.NumericLiteral)]
    [InlineData("", 26, 2, TokenType.Whitespace)]
    [InlineData("1237", 28, 4, TokenType.NumericLiteral)]
    [InlineData("", 32, 0, TokenType.EndOfSource)]
    public void ReadTokens_Returns_Numeric_Values(
        string expectedSymbol,
        int expectedOffset,
        int expectedLength,
        TokenType expectedTokens)
    {
        var source = "1.0 2.0 3.0 397.173 89 0.1  1237";
        var lexer = new Lexer(tokenizers, source);
        var token = lexer.NextToken();
        if (expectedOffset > 0)
        {
            while (token.Offset < expectedOffset && token.Type != TokenType.EndOfSource)
            {
                token = lexer.NextToken();
                if (token.IsError())
                {
                    Assert.Fail(token.Symbol);
                }
            }
        }

        Assert.True(token.IsMatch());
        Assert.Equal(expectedTokens, token.Type);
        Assert.Equal(expectedLength, token.Length);
        Assert.Equal(expectedSymbol, token.Symbol);
        Assert.Equal(expectedOffset, token.Offset);
    }
}
