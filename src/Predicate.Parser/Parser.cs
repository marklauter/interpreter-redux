using Luthor;
using Luthor.Tokens;
using Predicate.Parser.Exceptions;
using Predicate.Parser.Expressions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Predicate.Parser;

/*
https://bnfplayground.pauliankline.com/?bnf=%3Cpredicate%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cfromclause%3E%20%3Crequiredwhitespace%3E%20%3Cwhereclause%3E%0A%3Cfromclause%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%22from%22%20%3Crequiredwhitespace%3E%20%3Cidentifier%3E%0A%3Cwhereclause%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%22where%22%20%3Crequiredwhitespace%3E%20%3Ccondition%3E%0A%3Ccondition%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cterm%3E%20%7C%20%3Ccondition%3E%20%3Cor%3E%20%3Cterm%3E%0A%3Cor%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Crequiredwhitespace%3E%20%22%7C%7C%22%20%3Crequiredwhitespace%3E%20%0A%3Cterm%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cfactor%3E%20%7C%20%3Cterm%3E%20%3Cand%3E%20%3Cfactor%3E%0A%3Cand%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Crequiredwhitespace%3E%20%22%26%26%22%20%3Crequiredwhitespace%3E%20%0A%3Cfactor%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cproperty%3E%20%3Ccomparisonoperator%3E%20%3Cvalue%3E%20%7C%20%3Cparentheticalexpression%3E%0A%3Cparentheticalexpression%3E%20%20%20%3A%3A%3D%20%22(%22%20%3Coptionalwhitespace%3E%20%3Ccondition%3E%20%3Coptionalwhitespace%3E%20%22)%22%0A%3Crequiredwhitespace%3E%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cwhitespace%3E%2B%0A%3Coptionalwhitespace%3E%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cwhitespace%3E*%0A%3Cproperty%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cidentifier%3E%0A%3Ccomparisonoperator%3E%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Coptionalwhitespace%3E%20(%22%3D%3D%22%20%7C%20%22!%3D%22%20%7C%20%22%3C%22%20%7C%20%22%3E%22%20%7C%20%22%3C%3D%22%20%7C%20%22%3E%3D%22%20%7C%20%22-s%22%20%7C%20%22-e%22%20%7C%20%22-c%22)%20%3Coptionalwhitespace%3E%0A%3Cvalue%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cstringliteral%3E%20%7C%20%3Cnumericliteral%3E%0A%3Cidentifier%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20(%3Cuppercase%3E%20%7C%20%3Clowercase%3E)%2B%20%3Ccharacter%3E*%0A%3Cstringliteral%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cquote%3E%20%3Cstring%3E%20%3Cquote%3E%0A%3Cstring%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20(%3Ccharacter%3E%20%7C%20%3Cwhitespace%3E)*%0A%3Cquote%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%22%5C%22%22%0A%3Cnumericliteral%3E%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cpositivenumber%3E%20%7C%20%3Cnegativenumber%3E%0A%3Cnegativenumber%3E%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%22-%22%20%3Cpositivenumber%3E%0A%3Cpositivenumber%3E%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20(%220%22%20%7C%20%20%5B1-9%5D%20%3Cdigit%3E*)%20(%22.%22%20%3Cdigit%3E%2B%20)%3F%0A%3Cinteger%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cdigit%3E%2B%0A%3Ccharacter%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cuppercase%3E%20%7C%20%3Clowercase%3E%20%7C%20%3Cdigit%3E%0A%3Cuppercase%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%5BA-Z%5D%0A%3Clowercase%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%5Ba-z%5D%0A%3Cdigit%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%5B0-9%5D%0A%3Cwhitespace%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20(%22%20%22%20%7C%20%22%5Ct%22%20%7C%20%22%5Cr%22%20%7C%20%22%5Cn%22%20%7C%20%22%5Cr%5Cn%22)&name=predicate%20expression

<predicate>                 ::= <fromclause> <requiredwhitespace> <whereclause>
<fromclause>                ::= "from" <requiredwhitespace> <identifier>
<whereclause>               ::= "where" <requiredwhitespace> <logicalor>
<logicalor>                 ::= <logicaland>| <logicalor> <or> <term>
<logicaland>                ::= <comparison> | <logicaland><and> <comparison>
<comparison>                ::= <property> <comparisonoperator> <value> | <parentheticalexpression>
<parentheticalexpression>   ::= "(" <optionalwhitespace> <logicalor> <optionalwhitespace> ")"

<or>                        ::= <requiredwhitespace> "||" <requiredwhitespace> 
<and>                       ::= <requiredwhitespace> "&&" <requiredwhitespace> 
<requiredwhitespace>        ::= <whitespace>+
<optionalwhitespace>        ::= <whitespace>*
<property>                  ::= <identifier>
<comparisonoperator>        ::= <optionalwhitespace> ("==" | "!=" | "<" | ">" | "<=" | ">=" | "-s" | "-e" | "-c") <optionalwhitespace>
<value>                     ::= <stringliteral> | <numericliteral> | <booleanliteral>
<booleanliteral>            ::= "true" | "false"
<stringliteral>             ::= <quotedstring>
<numericliteral>            ::= <positivenumber> | <negativenumber>
<negativenumber>            ::= "-" <positivenumber>
<positivenumber>            ::= ("0" |  [1-9] <digit>*) ("." <digit>+ )?
<integer>                   ::= <digit>+
<quotedstring>              ::= <quote> <string> <quote>
<quote>                     ::= "\""
<identifier>                ::= (<uppercase> | <lowercase>)+ <character>*
<string>                    ::= (<character> | <whitespace>)*
<character>                 ::= <uppercase> | <lowercase> | <digit>
<uppercase>                 ::= [A-Z]
<lowercase>                 ::= [a-z]
<digit>                     ::= [0-9]
<whitespace>                ::= (" " | "\t" | "\r" | "\n" | "\r\n")
*/

public sealed class Parser(Tokenizers tokenizers)
{
    private readonly Tokenizers tokenizers = tokenizers
        ?? throw new ArgumentNullException(nameof(tokenizers));

    internal const string EndOfSourceError = "unexpected end of source";
    internal const string ParserError = "parser encountered errors";
    internal const string LexerError = "lexer encountered errors";

    public Expressions.Statement Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var lexer = new Lexer(tokenizers, source);
        var tokens = lexer
            .Tokens()
            .Where(token => !token.IsWhiteSpace());

        if (tokens.Any(t => t.IsError()))
        {
            var ex = new ParseException(LexerError);
            ex.Data.Add(
                "errors",
                tokens
                    .Where(t => t.IsError())
                    .Select(lexer.ReadSymbol)
                    .ToArray());
            throw ex;
        }

        if (tokens.First().IsEndOfSource())
        {
            throw new UnexpectedEndOfSourceException(EndOfSourceError);
        }

        var index = 0;
        return ParseStatement(
            lexer,
            tokens.ToArray().AsSpan(),
            ref index);
    }

    private static Statement ParseStatement(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        ref int index)
    {
        _ = ParseReservedWord(
            lexer,
            tokens,
            ReservedWords.From,
            ref index);

        var from = ParseIdentifier(
            lexer,
            tokens,
            ref index);

        var predicate = ParsePredicate(
            lexer,
            tokens,
            ref index);

        // todo: var skip = ParseSkip();
        // todo: var take = ParseTake();

        return new Statement(
            from,
            predicate,
            null,
            null);
    }

    private static Expression ParsePredicate(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        ref int index)
    {
        _ = ParseReservedWord(
            lexer,
            tokens,
            ReservedWords.Where,
            ref index);

        return ParseLogicalOr(
            lexer,
            tokens,
            ref index);
    }

    private static Expression ParseLogicalOr(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        ref int index)
    {
        var left = ParseLogicalAnd(
            lexer,
            tokens,
            ref index);

        var token = tokens[index];
        while (
            !token.IsEndOfSource()
            && token.IsOperator()
            && (LogicalOperator)lexer.ReadSymbol(token) == LogicalOperators.Or)
        {
            var op = ParseLogicalOperator(
                lexer,
                tokens,
                LogicalOperators.Or,
                ref index);

            var right = ParseLogicalAnd(
                lexer,
                tokens,
                ref index);

            left = new LogicalExpression(
                left,
                op,
                right);

            token = tokens[index];
        }

        return left;
    }

    private static Expression ParseLogicalAnd(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        ref int index)
    {
        var left = ParseComparison(
            lexer,
            tokens,
            ref index);

        var token = tokens[index];
        while (
            !token.IsEndOfSource()
            && token.IsOperator()
            && (LogicalOperator)lexer.ReadSymbol(token) == LogicalOperators.And)
        {
            var op = ParseLogicalOperator(
                lexer,
                tokens,
                LogicalOperators.And,
                ref index);

            var right = ParseComparison(
                lexer,
                tokens,
                ref index);

            left = new LogicalExpression(
                left,
                op,
                right);

            token = tokens[index];
        }

        return left;
    }

    [SuppressMessage("Style", "IDE0010:Add missing cases", Justification = "switch is complete")]
    private static Expression ParseComparison(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        ref int index)
    {
        var token = tokens[index];
        switch (token.Type)
        {
            case TokenType.OpenCircumfixDelimiter:
                return ParseParentheticalGroup(
                    lexer,
                    tokens,
                    ref index);

            case TokenType.Identifier:
                var left = ParseIdentifier(
                    lexer,
                    tokens,
                    ref index);

                var op = ParseComparisonOperator(
                    lexer,
                    tokens,
                    ref index);

                var right = ParseLiteral(
                    lexer,
                    tokens,
                    ref index);

                return new ComparisonExpression(left, op, right);

            default:
                throw new ParseException($"unexpected token {lexer.ReadSymbol(token)}. expected identifier or open parenthesis.");
        }
    }

    private static Expression ParseParentheticalGroup(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        ref int index)
    {
        throw new NotImplementedException(nameof(ParseParentheticalGroup));
    }

    private static ComparisonOperator ParseComparisonOperator(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        ref int index)
    {
        var token = tokens[index];
        CheckEndOfSource(token);

        if (token.IsOperator())
        {
            ++index;
            return (ComparisonOperator)lexer.ReadSymbol(token);
        }

        throw new ParseException($"unexpected token {lexer.ReadSymbol(token)}. expected {nameof(ComparisonOperator)}.");
    }

    private static LogicalOperator ParseLogicalOperator(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        LogicalOperators expectedOperator,
        ref int index)
    {
        var token = tokens[index];
        CheckEndOfSource(token);

        if (token.IsOperator())
        {
            var logicalOperator = (LogicalOperator)lexer.ReadSymbol(token);
            if (logicalOperator.Operator != expectedOperator)
            {
                throw new ParseException($"unexpected operator {logicalOperator.Operator}. expected {expectedOperator}.");
            }

            ++index;
            return logicalOperator;
        }

        throw new ParseException($"unexpected token {lexer.ReadSymbol(token)}. expected {TokenType.Operator}.");
    }

    private static ReservedWord ParseReservedWord(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        ReservedWords expectedWord,
        ref int index)
    {
        var token = tokens[index];
        CheckEndOfSource(token);

        if (token.IsReservedWord())
        {
            var reservedWord = (ReservedWord)lexer.ReadSymbol(token);
            if (reservedWord.Value != expectedWord)
            {
                throw new ParseException($"unexpected identifier {reservedWord.Value}. expected {expectedWord}.");
            }

            ++index;
            return reservedWord;
        }

        throw new ParseException($"unexpected token {lexer.ReadSymbol(token)}. expected {TokenType.ReservedWord}.");
    }

    private static Identifier ParseIdentifier(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        ref int index)
    {
        var token = tokens[index];
        CheckEndOfSource(token);

        if (token.IsIdentifier())
        {
            ++index;
            return (Identifier)lexer.ReadSymbol(token);
        }

        throw new ParseException($"unexpected token {lexer.ReadSymbol(token)}. expected {TokenType.Identifier}.");
    }

    [SuppressMessage("Style", "IDE0010:Add missing cases", Justification = "switch is complete")]
    private static Expression ParseLiteral(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        ref int index)
    {
        var token = tokens[index];
        CheckEndOfSource(token);

        if (token.IsLiteral())
        {
            var symbol = lexer.ReadSymbol(token);
            switch (token.Type)
            {
                case TokenType.BooleanLiteral:
                    ++index;
                    return (BooleanLiteral)symbol;
                case TokenType.StringLiteral:
                    ++index;
                    return (StringLiteral)symbol;
                case TokenType.NumericLiteral:
                    ++index;
                    return (NumericLiteral)symbol;
                default:
                    throw new ParseException($"unexpected token {lexer.ReadSymbol(token)}. expected {TokenType.BooleanLiteral} | {TokenType.StringLiteral} | {TokenType.NumericLiteral}.");
            }
        }

        throw new ParseException($"unexpected token {lexer.ReadSymbol(token)}. expected {TokenType.Literal}.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CheckEndOfSource(Token token)
    {
        if (token.IsEndOfSource())
        {
            throw new UnexpectedEndOfSourceException(EndOfSourceError);
        }
    }
}
