using Luthor.Context;
using Luthor.Spec;
using Luthor.Tokens;

namespace Luthor.Tests;

public sealed class LexerTests(LanguageSpecification language)
{
    private readonly LinguisticContext context = new(language ?? throw new ArgumentNullException(nameof(language)));

    [Theory]
    [InlineData("2a", 0, 0, TokenType.Error)]

    [InlineData(" ", 0, 1, TokenType.Whitespace)]
    [InlineData("  ", 0, 2, TokenType.Whitespace)]
    [InlineData(" x", 0, 1, TokenType.Whitespace)]

    [InlineData("", 0, 0, TokenType.Eof)]

    [InlineData("\r", 0, 1, TokenType.NewLine)]
    [InlineData("\n", 0, 1, TokenType.NewLine)]
    [InlineData("\r\n", 0, 2, TokenType.NewLine)]

    [InlineData("if", 0, 2, TokenType.ReservedWord)]
    [InlineData("else", 0, 4, TokenType.ReservedWord)]
    [InlineData("let", 0, 3, TokenType.ReservedWord)]

    [InlineData("x", 0, 1, TokenType.Identifier)]
    [InlineData("xy", 0, 2, TokenType.Identifier)]
    [InlineData("iffy", 0, 4, TokenType.Identifier)]
    [InlineData("letMe", 0, 5, TokenType.Identifier)]

    [InlineData("1", 0, 1, TokenType.NumericLiteral)]
    [InlineData("12", 0, 2, TokenType.NumericLiteral)]
    [InlineData("12.3", 0, 4, TokenType.NumericLiteral)]

    [InlineData(@"""hello""", 0, 7, TokenType.StringLiteral)]
    [InlineData(@"""hello \""world\""""", 0, 17, TokenType.StringLiteral)]

    [InlineData("true", 0, 4, TokenType.BooleanLiteral)]
    [InlineData("false", 0, 5, TokenType.BooleanLiteral)]

    [InlineData("'a'", 0, 3, TokenType.CharacterLiteral)]
    [InlineData("'a '", 0, 0, TokenType.Error)]

    [InlineData("// comment", 0, 10, TokenType.Comment)]
    [InlineData("## comment", 0, 10, TokenType.Comment)]

    [InlineData(";", 0, 1, TokenType.Punctuation)]
    [InlineData(",", 0, 1, TokenType.Punctuation)]

    [InlineData("x+y", 1, 1, TokenType.InfixOperator)]
    [InlineData("x + y", 2, 1, TokenType.InfixOperator)]
    [InlineData("x<=y", 1, 2, TokenType.InfixOperator)]
    [InlineData("x <= y", 2, 2, TokenType.InfixOperator)]

    [InlineData("!i", 0, 1, TokenType.PrefixOperator)]
    [InlineData("! i", 0, 1, TokenType.PrefixOperator)]
    [InlineData("++i", 0, 2, TokenType.PrefixOperator)]
    [InlineData("++ i", 0, 2, TokenType.PrefixOperator)]
    [InlineData("i++", 1, 2, TokenType.PostfixOperator)]
    [InlineData("i ++", 2, 2, TokenType.PostfixOperator)]
    // [InlineData(" ", 2, TokenType.CircumfixOperator)]
    public void Test(string source, int position, int length, TokenType tokenType)
    {
        var lexer = new Lexer(context, source);
        var token = lexer.ReadNextToken();
        for (var i = 0; i < position; ++i)
        {
            token = lexer.ReadNextToken();
        }

        Assert.Equal(tokenType, token.Type);
        Assert.Equal(length, token.Length);
    }

    [Theory]
    [InlineData(0, "if", 0, 2, TokenType.ReservedWord)]
    [InlineData(1, "", 2, 1, TokenType.Whitespace)]
    [InlineData(2, "(", 3, 1, TokenType.Punctuation)]
    [InlineData(3, "a", 4, 1, TokenType.Identifier)]
    [InlineData(4, "", 5, 1, TokenType.Whitespace)]
    [InlineData(5, "+", 6, 1, TokenType.InfixOperator)]
    [InlineData(6, "", 7, 1, TokenType.Whitespace)]
    [InlineData(7, "b", 8, 1, TokenType.Identifier)]
    [InlineData(8, ")", 9, 1, TokenType.Punctuation)]
    [InlineData(9, "", 10, 1, TokenType.Whitespace)]
    [InlineData(10, "{", 11, 1, TokenType.Punctuation)]
    [InlineData(11, "", 12, 1, TokenType.Whitespace)]
    [InlineData(12, "let", 13, 3, TokenType.ReservedWord)]
    [InlineData(13, "", 16, 1, TokenType.Whitespace)]
    [InlineData(14, "c", 17, 1, TokenType.Identifier)]
    [InlineData(15, "", 18, 1, TokenType.Whitespace)]
    [InlineData(16, "=", 19, 1, TokenType.InfixOperator)]
    [InlineData(17, "", 20, 1, TokenType.Whitespace)]
    [InlineData(18, "a", 21, 1, TokenType.Identifier)]
    [InlineData(19, "", 22, 1, TokenType.Whitespace)]
    [InlineData(20, "+", 23, 1, TokenType.InfixOperator)]
    [InlineData(21, "", 24, 1, TokenType.Whitespace)]
    [InlineData(22, "b", 25, 1, TokenType.Identifier)]
    [InlineData(23, "^", 26, 1, TokenType.InfixOperator)]
    [InlineData(24, "2", 27, 1, TokenType.NumericLiteral)]
    [InlineData(25, ";", 28, 1, TokenType.Punctuation)]
    [InlineData(26, "", 29, 1, TokenType.Whitespace)]
    [InlineData(27, "}", 30, 1, TokenType.Punctuation)]
    [InlineData(28, "", 31, 0, TokenType.Eof)]
    public void ReadTokens_Returns_TokenTable(int index, string value, int offset, int length, TokenType tokenType)
    {
        var source = "if (a + b) { let c = a + b^2; }";
        var lexer = new Lexer(context, source);

        var tokens = lexer
            .ReadTokens()
            .ToArray();

        Assert.Equal(29, tokens.Length);

        Assert.Equal(value, tokens[index].Value);
        Assert.Equal(offset, tokens[index].Offset);
        Assert.Equal(length, tokens[index].Length);
        Assert.Equal(tokenType, tokens[index].Type);
    }

    [Theory]
    [InlineData(0, "1.0", 0, 3, TokenType.NumericLiteral)]
    [InlineData(1, "", 3, 1, TokenType.Whitespace)]
    [InlineData(2, "2.0", 4, 3, TokenType.NumericLiteral)]
    [InlineData(3, "", 7, 1, TokenType.Whitespace)]
    [InlineData(4, "3.0", 8, 3, TokenType.NumericLiteral)]
    [InlineData(5, "", 11, 1, TokenType.Whitespace)]
    [InlineData(6, "397.173", 12, 7, TokenType.NumericLiteral)]
    [InlineData(7, "", 19, 1, TokenType.Whitespace)]
    [InlineData(8, "89", 20, 2, TokenType.NumericLiteral)]
    [InlineData(9, "", 22, 1, TokenType.Whitespace)]
    [InlineData(10, "0.1", 23, 3, TokenType.NumericLiteral)]
    [InlineData(11, "", 26, 2, TokenType.Whitespace)]
    [InlineData(12, "1237", 28, 4, TokenType.NumericLiteral)]
    [InlineData(13, "", 32, 0, TokenType.Eof)]
    public void ReadTokens_Returns_Numeric_Values(int index, string value, int offset, int length, TokenType tokenType)
    {
        var source = "1.0 2.0 3.0 397.173 89 0.1  1237";
        var lexer = new Lexer(context, source);

        var tokens = lexer
            .ReadTokens()
            .ToArray();

        Assert.Equal(14, tokens.Length);

        Assert.Equal(value, tokens[index].Value);
        Assert.Equal(offset, tokens[index].Offset);
        Assert.Equal(length, tokens[index].Length);
        Assert.Equal(tokenType, tokens[index].Type);
    }
}
