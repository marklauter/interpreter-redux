using Luthor.Context;

namespace Luthor.Tests;

public sealed class LexerTests(LexicalContext context)
{
    private readonly LexicalContext context = context ?? throw new ArgumentNullException(nameof(context));

    [Theory]
    [InlineData("2a", @"{""line"": 1, ""column"": 0}", 0, 0, Tokens.Error)]

    [InlineData(" ", "", 0, 1, Tokens.Whitespace)]
    [InlineData("  ", "", 0, 2, Tokens.Whitespace)]
    [InlineData(" x", "", 0, 1, Tokens.Whitespace)]

    [InlineData("", "", 0, 0, Tokens.EndOfSource)]

    [InlineData("\r", "", 0, 1, Tokens.NewLine)]
    [InlineData("\n", "", 0, 1, Tokens.NewLine)]
    [InlineData("\r\n", "", 0, 2, Tokens.NewLine)]

    [InlineData("if", "if", 0, 2, Tokens.ReservedWord)]
    [InlineData("else", "else", 0, 4, Tokens.ReservedWord)]
    [InlineData("let", "let", 0, 3, Tokens.ReservedWord)]

    [InlineData("x", "x", 0, 1, Tokens.Identifier)]
    [InlineData("xy", "xy", 0, 2, Tokens.Identifier)]
    [InlineData("iffy", "iffy", 0, 4, Tokens.Identifier)]
    [InlineData("letMe", "letMe", 0, 5, Tokens.Identifier)]
    [InlineData("miffed", "miffed", 0, 6, Tokens.Identifier)]
    [InlineData("bif", "bif", 0, 3, Tokens.Identifier)]

    [InlineData("1", "1", 0, 1, Tokens.NumericLiteral)]
    [InlineData("12", "12", 0, 2, Tokens.NumericLiteral)]
    [InlineData("123", "123", 0, 3, Tokens.NumericLiteral)]
    [InlineData("1234", "1234", 0, 4, Tokens.NumericLiteral)]
    [InlineData("1234.0", "1234.0", 0, 6, Tokens.NumericLiteral)]
    [InlineData("123.40", "123.40", 0, 6, Tokens.NumericLiteral)]
    [InlineData("12.340", "12.340", 0, 6, Tokens.NumericLiteral)]
    [InlineData("1.2340", "1.2340", 0, 6, Tokens.NumericLiteral)]
    [InlineData("0.1234", "0.1234", 0, 6, Tokens.NumericLiteral)]
    [InlineData("1234. ", @"{""line"": 1, ""column"": 0}", 0, 0, Tokens.Error)]

    [InlineData(@"""hello""", @"""hello""", 0, 7, Tokens.StringLiteral)]
    [InlineData(@"""hello \""world\""""", @"""hello \""world\""""", 0, 17, Tokens.StringLiteral)]
    [InlineData(@"""hello", @"{""line"": 1, ""column"": 0}", 0, 0, Tokens.Error)]

    [InlineData("true", "true", 0, 4, Tokens.BooleanLiteral)]
    [InlineData("false", "false", 0, 5, Tokens.BooleanLiteral)]

    [InlineData("'a'", "'a'", 0, 3, Tokens.CharacterLiteral)]
    [InlineData("'a '", @"{""line"": 1, ""column"": 0}", 0, 0, Tokens.Error)]

    [InlineData("// comment", "// comment", 0, 10, Tokens.Comment)]
    [InlineData("# comment", "# comment", 0, 9, Tokens.Comment)]

    [InlineData(";", ";", 0, 1, Tokens.InfixDelimiter)]
    [InlineData(",", ",", 0, 1, Tokens.InfixDelimiter)]

    [InlineData("x+y", "+", 1, 1, Tokens.Operator)]
    [InlineData("x + y", "+", 2, 1, Tokens.Operator)]
    [InlineData("x<=y", "<=", 1, 2, Tokens.Operator)]
    [InlineData("x <= y", "<=", 2, 2, Tokens.Operator)]
    [InlineData("i<1", "<", 1, 1, Tokens.Operator)]

    [InlineData("!i", "!", 0, 1, Tokens.Operator)]
    [InlineData("! i", "!", 0, 1, Tokens.Operator)]
    [InlineData("++i", "++", 0, 2, Tokens.Operator)]
    [InlineData("++ i", "++", 0, 2, Tokens.Operator)]
    [InlineData("i++", "++", 1, 2, Tokens.Operator)]
    [InlineData("i ++", "++", 2, 2, Tokens.Operator)]
    public void Returns_Expected_Token(
        string source,
        string expectedSymbol,
        int expectedOffset,
        int expectedLength,
        Tokens expectedTokens)
    {
        var lexer = new Lexer(context);
        var result = lexer.ReadToken(source);
        if (expectedOffset > 0)
        {
            while (result.NextOffset <= expectedOffset && result.Token.Type != Tokens.EndOfSource)
            {
                result = lexer.ReadToken(source, result.NextOffset, result.LastNewLineOffset, result.LineNumber);
            }
        }

        _ = result.Token;
        Assert.Equal(expectedTokens, result.Token.Type);
        Assert.Equal(expectedLength, result.Token.Length);
        Assert.Equal(expectedSymbol, result.Token.Symbol);
        Assert.Equal(expectedOffset, result.Token.Offset);
    }

    [Theory]
    [InlineData("if", 0, 2, Tokens.ReservedWord)]
    [InlineData("", 2, 1, Tokens.Whitespace)]
    [InlineData("(", 3, 1, Tokens.OpenCircumfixDelimiter)]
    [InlineData("a", 4, 1, Tokens.Identifier)]
    [InlineData("", 5, 1, Tokens.Whitespace)]
    [InlineData("+", 6, 1, Tokens.Operator)]
    [InlineData("", 7, 1, Tokens.Whitespace)]
    [InlineData("b", 8, 1, Tokens.Identifier)]
    [InlineData(")", 9, 1, Tokens.CloseCircumfixDelimiter)]
    [InlineData("", 10, 1, Tokens.Whitespace)]
    [InlineData("{", 11, 1, Tokens.OpenCircumfixDelimiter)]
    [InlineData("", 12, 1, Tokens.Whitespace)]
    [InlineData("let", 13, 3, Tokens.ReservedWord)]
    [InlineData("", 16, 1, Tokens.Whitespace)]
    [InlineData("c", 17, 1, Tokens.Identifier)]
    [InlineData("", 18, 1, Tokens.Whitespace)]
    [InlineData("=", 19, 1, Tokens.Operator)]
    [InlineData("", 20, 1, Tokens.Whitespace)]
    [InlineData("a", 21, 1, Tokens.Identifier)]
    [InlineData("", 22, 1, Tokens.Whitespace)]
    [InlineData("+", 23, 1, Tokens.Operator)]
    [InlineData("", 24, 1, Tokens.Whitespace)]
    [InlineData("b", 25, 1, Tokens.Identifier)]
    [InlineData("^", 26, 1, Tokens.Operator)]
    [InlineData("2", 27, 1, Tokens.NumericLiteral)]
    [InlineData(";", 28, 1, Tokens.InfixDelimiter)]
    [InlineData("", 29, 1, Tokens.Whitespace)]
    [InlineData("}", 30, 1, Tokens.CloseCircumfixDelimiter)]
    [InlineData("", 31, 0, Tokens.EndOfSource)]
    public void Returns_Expected_Token_From_MultiToken_Source(string expectedSymbol, int expectedOffset, int expectedLength, Tokens expectedTokens)
    {
        var source = "if (a + b) { let c = a + b^2; }";
        var lexer = new Lexer(context);

        var response = lexer.ReadToken(source);
        if (expectedOffset > 0)
        {
            while (response.NextOffset <= expectedOffset && response.Token.Type != Tokens.EndOfSource)
            {
                response = lexer.ReadToken(source, response.NextOffset, response.LastNewLineOffset, response.LineNumber);
            }
        }

        Assert.Equal(expectedTokens, response.Token.Type);
        Assert.Equal(expectedLength, response.Token.Length);
        Assert.Equal(expectedSymbol, response.Token.Symbol);
        Assert.Equal(expectedOffset, response.Token.Offset);
    }

    [Theory]
    [InlineData("1.0", 0, 3, Tokens.NumericLiteral)]
    [InlineData("", 3, 1, Tokens.Whitespace)]
    [InlineData("2.0", 4, 3, Tokens.NumericLiteral)]
    [InlineData("", 7, 1, Tokens.Whitespace)]
    [InlineData("3.0", 8, 3, Tokens.NumericLiteral)]
    [InlineData("", 11, 1, Tokens.Whitespace)]
    [InlineData("397.173", 12, 7, Tokens.NumericLiteral)]
    [InlineData("", 19, 1, Tokens.Whitespace)]
    [InlineData("89", 20, 2, Tokens.NumericLiteral)]
    [InlineData("", 22, 1, Tokens.Whitespace)]
    [InlineData("0.1", 23, 3, Tokens.NumericLiteral)]
    [InlineData("", 26, 2, Tokens.Whitespace)]
    [InlineData("1237", 28, 4, Tokens.NumericLiteral)]
    [InlineData("", 32, 0, Tokens.EndOfSource)]
    public void ReadTokens_Returns_Numeric_Values(
        string expectedSymbol,
        int expectedOffset,
        int expectedLength,
        Tokens expectedTokens)
    {
        var source = "1.0 2.0 3.0 397.173 89 0.1  1237";
        var lexer = new Lexer(context);

        var result = lexer.ReadToken(source);
        if (expectedOffset > 0)
        {
            while (result.NextOffset <= expectedOffset && result.Token.Type != Tokens.EndOfSource)
            {
                result = lexer.ReadToken(source, result.NextOffset, result.LastNewLineOffset, result.LineNumber);
            }
        }

        Assert.Equal(expectedTokens, result.Token.Type);
        Assert.Equal(expectedLength, result.Token.Length);
        Assert.Equal(expectedSymbol, result.Token.Symbol);
        Assert.Equal(expectedOffset, result.Token.Offset);
    }
}
