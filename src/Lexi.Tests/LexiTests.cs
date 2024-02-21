using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Lexi.Tests;

[ExcludeFromCodeCoverage]
public sealed class LexiTests(Lexer lexer)
{
    public Lexer Lexer { get; } = lexer
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
        var result = lexer.NextMatch(new Script(source));
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
            var result = lexer.NextMatch(script);
            Assert.Equal(expectedId[i], result.Symbol.TokenId);
            var symbol = result.Symbol;
            Assert.Equal(symbols[i], result.Script.ReadSymbol(in symbol));
            script = result.Script;
        }
    }

    [Fact]
    public void LexOrder()
    {
        var source = "from Address where Street startswith \"Cypress\" and (City = \"Tampa\" or City = \"Miami\")";
        var lexer = LexerBuilder
            .Create(RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
            .MatchKeyword($"{nameof(TokenIds.FROM)}", TokenIds.FROM)
            .MatchKeyword($"{nameof(TokenIds.WHERE)}", TokenIds.WHERE)
            .MatchKeyword($"{nameof(TokenIds.SKIP)}", TokenIds.SKIP)
            .MatchKeyword($"{nameof(TokenIds.TAKE)}", TokenIds.TAKE)
            .MatchOperator("contains|c", TokenIds.CONTAINS)
            .MatchOperator("startswith|sw", TokenIds.STARTS_WITH)
            .MatchOperator("endswith|ew", TokenIds.ENDS_WITH)
            .MatchOperator(@"and|&&", TokenIds.LOGICAL_AND)
            .MatchOperator(@"or|\|\|", TokenIds.LOGICAL_OR)
            .MatchIdentifier(TokenIds.IDENTIFIER)
            .MatchBooleanTrueLiteral("true", TokenIds.TRUE)
            .MatchBooleanFalseLiteral("false", TokenIds.FALSE)
            .MatchIntegerLiteral(TokenIds.INTEGER_LITERAL)
            .MatchFloatingPointLiteral(TokenIds.FLOATING_POINT_LITERAL)
            .MatchScientificNotationLiteral(TokenIds.SCIENTIFIC_NOTATION_LITERAL)
            .MatchStringLiteral(TokenIds.STRING_LITERAL)
            .MatchCharacterLiteral(TokenIds.CHAR_LITERAL)
            .MatchOpeningCircumfixDelimiter(@"\(", TokenIds.OPEN_PARENTHESIS)
            .MatchClosingCircumfixDelimiter(@"\)", TokenIds.CLOSE_PARENTHESIS)
            .MatchOperator("=|==", TokenIds.EQUAL)
            .MatchOperator(">", TokenIds.GREATER_THAN)
            .MatchOperator(">=", TokenIds.GREATER_THAN_OR_EQUAL)
            .MatchOperator("<", TokenIds.LESS_THAN)
            .MatchOperator("<=", TokenIds.LESS_THAN_OR_EQUAL)
            .MatchOperator("!=", TokenIds.NOT_EQUAL)
            .Build();

        var match = lexer.NextMatch(source);
        Assert.Equal(TokenIds.FROM, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.IDENTIFIER, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.WHERE, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.IDENTIFIER, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.STARTS_WITH, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.STRING_LITERAL, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.LOGICAL_AND, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.OPEN_PARENTHESIS, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.IDENTIFIER, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.EQUAL, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.STRING_LITERAL, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.LOGICAL_OR, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.IDENTIFIER, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.EQUAL, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.STRING_LITERAL, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.CLOSE_PARENTHESIS, match.Symbol.TokenId);
    }

    [Fact]
    public void LexOrder2()
    {
        var source = "(City = \"Tampa\" or City = \"Miami\")";

        var lexer = LexerBuilder
            .Create(RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
            .MatchOperator("(?:contains|c)", TokenIds.CONTAINS)
            .MatchOperator("and|&&", TokenIds.LOGICAL_AND)
            .MatchOperator(@"or|\|\|", TokenIds.LOGICAL_OR)
            .MatchIdentifier(TokenIds.IDENTIFIER)
            .MatchStringLiteral(TokenIds.STRING_LITERAL)
            .MatchCharacterLiteral(TokenIds.CHAR_LITERAL)
            .MatchOpeningCircumfixDelimiter(@"\(", TokenIds.OPEN_PARENTHESIS)
            .MatchClosingCircumfixDelimiter(@"\)", TokenIds.CLOSE_PARENTHESIS)
            .MatchOperator("=|==", TokenIds.EQUAL)
            .Build();

        var match = lexer.NextMatch(source);
        Assert.Equal(TokenIds.OPEN_PARENTHESIS, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.IDENTIFIER, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.EQUAL, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.STRING_LITERAL, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.LOGICAL_OR, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.IDENTIFIER, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.EQUAL, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.STRING_LITERAL, match.Symbol.TokenId);

        match = lexer.NextMatch(match.Script);
        Assert.Equal(TokenIds.CLOSE_PARENTHESIS, match.Symbol.TokenId);
    }

    internal sealed class TokenIds
    {
        // literals
        public const int FALSE = 0;
        public const int TRUE = 1;
        public const int FLOATING_POINT_LITERAL = 2;
        public const int INTEGER_LITERAL = 3;
        public const int SCIENTIFIC_NOTATION_LITERAL = 4;
        public const int STRING_LITERAL = 5;
        public const int CHAR_LITERAL = 6;

        // delimiters
        public const int OPEN_PARENTHESIS = '('; // 40
        public const int CLOSE_PARENTHESIS = ')'; // 41

        // comparison operators
        public const int EQUAL = '='; // 61
        public const int GREATER_THAN = '>'; // 62
        public const int LESS_THAN = '<'; // 60
        public const int NOT_EQUAL = 400;
        public const int LESS_THAN_OR_EQUAL = 401;
        public const int GREATER_THAN_OR_EQUAL = 402;
        public const int STARTS_WITH = 405;
        public const int ENDS_WITH = 406;
        public const int CONTAINS = 407;

        // logical operators
        public const int LOGICAL_AND = 403;
        public const int LOGICAL_OR = 404;

        // names
        public const int IDENTIFIER = 500;

        // keywords
        public const int FROM = 300;
        public const int WHERE = 301;
        public const int SKIP = 302;
        public const int TAKE = 303;
    }
}
