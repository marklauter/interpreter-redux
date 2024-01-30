//using Luthor.Context;
//using Luthor.Tokens;

//namespace Luthor.Tests;

//public sealed class FLexerTests(LinguisticContext context)
//{
//    private readonly LinguisticContext context = context ?? throw new ArgumentNullException(nameof(context));

//    [Theory]
//    [InlineData("2a", 0, 0, TokenType.Error)]

//    [InlineData(" ", 0, 1, TokenType.Whitespace)]
//    [InlineData("  ", 0, 2, TokenType.Whitespace)]
//    [InlineData(" x", 0, 1, TokenType.Whitespace)]

//    [InlineData("", 0, 0, TokenType.EndOfSource)]

//    [InlineData("\r", 0, 1, TokenType.NewLine)]
//    [InlineData("\n", 0, 1, TokenType.NewLine)]
//    [InlineData("\r\n", 0, 2, TokenType.NewLine)]

//    [InlineData("if", 0, 2, TokenType.ReservedWord)]
//    [InlineData("else", 0, 4, TokenType.ReservedWord)]
//    [InlineData("let", 0, 3, TokenType.ReservedWord)]

//    [InlineData("x", 0, 1, TokenType.Identifier)]
//    [InlineData("xy", 0, 2, TokenType.Identifier)]
//    [InlineData("iffy", 0, 4, TokenType.Identifier)]
//    [InlineData("letMe", 0, 5, TokenType.Identifier)]

//    [InlineData("1", 0, 1, TokenType.NumericLiteral)]
//    [InlineData("12", 0, 2, TokenType.NumericLiteral)]
//    [InlineData("12.3", 0, 4, TokenType.NumericLiteral)]

//    [InlineData(@"""hello""", 0, 7, TokenType.StringLiteral)]
//    [InlineData(@"""hello \""world\""""", 0, 17, TokenType.StringLiteral)]

//    [InlineData("true", 0, 4, TokenType.BooleanLiteral)]
//    [InlineData("false", 0, 5, TokenType.BooleanLiteral)]

//    [InlineData("'a'", 0, 3, TokenType.CharacterLiteral)]
//    [InlineData("'a '", 0, 0, TokenType.Error)]

//    [InlineData("// comment", 0, 10, TokenType.Comment)]
//    [InlineData("## comment", 0, 10, TokenType.Comment)]

//    [InlineData(";", 0, 1, TokenType.Punctuation)]
//    [InlineData(",", 0, 1, TokenType.Punctuation)]

//    [InlineData("x+y", 1, 1, TokenType.InfixOperator)]
//    [InlineData("x + y", 2, 1, TokenType.InfixOperator)]
//    [InlineData("x<=y", 1, 2, TokenType.InfixOperator)]
//    [InlineData("x <= y", 2, 2, TokenType.InfixOperator)]

//    [InlineData("!i", 0, 1, TokenType.PrefixOperator)]
//    [InlineData("! i", 0, 1, TokenType.PrefixOperator)]
//    [InlineData("++i", 0, 2, TokenType.PrefixOperator)]
//    [InlineData("++ i", 0, 2, TokenType.PrefixOperator)]
//    [InlineData("i++", 1, 2, TokenType.PostfixOperator)]
//    [InlineData("i ++", 2, 2, TokenType.PostfixOperator)]
//    // [InlineData(" ", 2, TokenType.CircumfixOperator)]
//    public void Returns_Expected_Token(string source, int startingPosition, int length, TokenType tokenType)
//    {
//        var lexer = new FLexer(context);
//        var response = lexer.ReadToken(source);
//        if (startingPosition > 0)
//        {
//            while (response.Position <= startingPosition && response.Token.Type != TokenType.EndOfSource)
//            {
//                response = lexer.ReadToken(source, response.Position, response.LastNewLineOffset, response.LineNumber);
//            }
//        }

//        var token = response.Token;
//        Assert.Equal(tokenType, token.Type);
//        Assert.Equal(length, token.Length);
//    }

//    [Theory]
//    [InlineData("if", 0, 2, TokenType.ReservedWord)]
//    [InlineData("", 2, 1, TokenType.Whitespace)]
//    [InlineData("(", 3, 1, TokenType.Punctuation)]
//    [InlineData("a", 4, 1, TokenType.Identifier)]
//    [InlineData("", 5, 1, TokenType.Whitespace)]
//    [InlineData("+", 6, 1, TokenType.InfixOperator)]
//    [InlineData("", 7, 1, TokenType.Whitespace)]
//    [InlineData("b", 8, 1, TokenType.Identifier)]
//    [InlineData(")", 9, 1, TokenType.Punctuation)]
//    [InlineData("", 10, 1, TokenType.Whitespace)]
//    [InlineData("{", 11, 1, TokenType.Punctuation)]
//    [InlineData("", 12, 1, TokenType.Whitespace)]
//    [InlineData("let", 13, 3, TokenType.ReservedWord)]
//    [InlineData("", 16, 1, TokenType.Whitespace)]
//    [InlineData("c", 17, 1, TokenType.Identifier)]
//    [InlineData("", 18, 1, TokenType.Whitespace)]
//    [InlineData("=", 19, 1, TokenType.InfixOperator)]
//    [InlineData("", 20, 1, TokenType.Whitespace)]
//    [InlineData("a", 21, 1, TokenType.Identifier)]
//    [InlineData("", 22, 1, TokenType.Whitespace)]
//    [InlineData("+", 23, 1, TokenType.InfixOperator)]
//    [InlineData("", 24, 1, TokenType.Whitespace)]
//    [InlineData("b", 25, 1, TokenType.Identifier)]
//    [InlineData("^", 26, 1, TokenType.InfixOperator)]
//    [InlineData("2", 27, 1, TokenType.NumericLiteral)]
//    [InlineData(";", 28, 1, TokenType.Punctuation)]
//    [InlineData("", 29, 1, TokenType.Whitespace)]
//    [InlineData("}", 30, 1, TokenType.Punctuation)]
//    [InlineData("", 31, 0, TokenType.EndOfSource)]
//    public void Returns_Expected_Token_From_MultiToken_Source(string expectedValue, int expectedOffset, int expectedLength, TokenType expectedTokenType)
//    {
//        var source = "if (a + b) { let c = a + b^2; }";
//        var lexer = new FLexer(context);

//        var response = lexer.ReadToken(source);
//        if (expectedOffset > 0)
//        {
//            while (response.Position <= expectedOffset && response.Token.Type != TokenType.EndOfSource)
//            {
//                response = lexer.ReadToken(source, response.Position, response.LastNewLineOffset, response.LineNumber);
//            }
//        }

//        Assert.Equal(expectedTokenType, response.Token.Type);
//        Assert.Equal(expectedLength, response.Token.Length);
//        Assert.Equal(expectedValue, response.Token.Value);
//        Assert.Equal(expectedOffset, response.Token.Offset);
//    }

//    [Theory]
//    [InlineData("1.0", 0, 3, TokenType.NumericLiteral)]
//    [InlineData("", 3, 1, TokenType.Whitespace)]
//    [InlineData("2.0", 4, 3, TokenType.NumericLiteral)]
//    [InlineData("", 7, 1, TokenType.Whitespace)]
//    [InlineData("3.0", 8, 3, TokenType.NumericLiteral)]
//    [InlineData("", 11, 1, TokenType.Whitespace)]
//    [InlineData("397.173", 12, 7, TokenType.NumericLiteral)]
//    [InlineData("", 19, 1, TokenType.Whitespace)]
//    [InlineData("89", 20, 2, TokenType.NumericLiteral)]
//    [InlineData("", 22, 1, TokenType.Whitespace)]
//    [InlineData("0.1", 23, 3, TokenType.NumericLiteral)]
//    [InlineData("", 26, 2, TokenType.Whitespace)]
//    [InlineData("1237", 28, 4, TokenType.NumericLiteral)]
//    [InlineData("", 32, 0, TokenType.EndOfSource)]
//    public void ReadTokens_Returns_Numeric_Values(string expectedValue, int expectedOffset, int expectedLength, TokenType expectedTokenType)
//    {
//        var source = "1.0 2.0 3.0 397.173 89 0.1  1237";
//        var lexer = new FLexer(context);

//        var response = lexer.ReadToken(source);
//        if (expectedOffset > 0)
//        {
//            while (response.Position <= expectedOffset && response.Token.Type != TokenType.EndOfSource)
//            {
//                response = lexer.ReadToken(source, response.Position, response.LastNewLineOffset, response.LineNumber);
//            }
//        }

//        Assert.Equal(expectedTokenType, response.Token.Type);
//        Assert.Equal(expectedLength, response.Token.Length);
//        Assert.Equal(expectedValue, response.Token.Value);
//        Assert.Equal(expectedOffset, response.Token.Offset);
//    }
//}
