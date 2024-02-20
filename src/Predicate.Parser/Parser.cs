using Lexi;
using Predicate.Parser.Exceptions;
using Predicate.Parser.Expressions;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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

public sealed class Parser(Lexer lexer)
{
    private readonly Lexer lexer = lexer
        ?? throw new ArgumentNullException(nameof(lexer));

    private readonly ref struct ParseResult<T>(
        T expression,
        MatchResult matchResult)
        where T : Expression
    {
        public readonly T Expression = expression;
        public readonly MatchResult MatchResult = matchResult;
    }

    // todo: lexer returns tokens tagged as error, so we can add CheckError method for better error reporting
    public Statement Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return ParseStatement(new Script(source));
    }

    private Statement ParseStatement(Script script)
    {
        var matchResult = lexer.NextMatch(script);
        _ = ParseKeyword(in matchResult, Keywords.From);

        matchResult = lexer.NextMatch(matchResult);
        var identifier = ParseIdentifier(in matchResult);

        matchResult = lexer.NextMatch(matchResult);
        _ = ParseKeyword(in matchResult, Keywords.Where);

        var predicate = ParsePredicate(matchResult);

        var (skip, take) = ParseSkipTake(predicate.MatchResult);

        return new Statement(
            identifier,
            predicate.Expression,
            skip,
            take);
    }

    private ParseResult<Expression> ParsePredicate(MatchResult matchResult)
    {
        var left = ParseLogicalAnd(matchResult);

        matchResult = lexer.NextMatch(matchResult);

        while (!matchResult.Script.IsEndOfSource()
            && matchResult.Symbol.IsOperator()
            && matchResult.Symbol.TokenId == TokenIds.LOGICAL_OR)
        {
            var right = ParseLogicalAnd(matchResult);

            left = new(new LogicalExpression(
                left.Expression,
                LogicalOperators.Or,
                right.Expression),
                right.MatchResult);

            matchResult = lexer.NextMatch(right.MatchResult);
        }

        return left;
    }

    private ParseResult<Expression> ParseLogicalAnd(MatchResult matchResult)
    {
        var left = ParseComparison(matchResult);

        matchResult = lexer.NextMatch(left.MatchResult);
        while (!matchResult.Script.IsEndOfSource()
            && matchResult.Symbol.IsOperator()
            && matchResult.Symbol.TokenId == TokenIds.LOGICAL_AND)
        {
            var right = ParseComparison(matchResult);

            left = new(new LogicalExpression(
                left.Expression,
                LogicalOperators.And,
                right.Expression),
                right.MatchResult);

            matchResult = lexer.NextMatch(right.MatchResult);
        }

        return left;
    }

    [SuppressMessage("Style", "IDE0010:Add missing cases", Justification = "switch is complete")]
    private ParseResult<Expression> ParseComparison(MatchResult matchResult)
    {
        CheckEndOfSource(in matchResult);

        matchResult = lexer.NextMatch(matchResult);
        switch (matchResult.Symbol.Token)
        {
            case Tokens.Identifier:
                var left = ParseIdentifier(in matchResult);

                matchResult = lexer.NextMatch(matchResult);
                var op = ParseComparisonOperator(in matchResult);

                matchResult = lexer.NextMatch(matchResult);
                var right = ParseLiteral(in matchResult);

                return new(new ComparisonExpression(
                    left,
                    op.Value,
                    right),
                    matchResult);

            case Tokens.OpenCircumfixDelimiter:
                return ParseParentheticalGroup(matchResult);

            default:
                throw new UnexpectedTokenException($"unexpected token '{matchResult.Script.ReadSymbol(in matchResult.Symbol)}' at offset {matchResult.Script.Offset}. expected {nameof(Tokens.Identifier)} | {nameof(Tokens.OpenCircumfixDelimiter)}.");
        }
    }

    private ParseResult<Expression> ParseParentheticalGroup(
        MatchResult matchResult)
    {
        var predicate = ParsePredicate(matchResult);

        matchResult = lexer.NextMatch(predicate.MatchResult);
        CheckEndOfSource(in matchResult);

        return matchResult.Symbol.IsCloseCircumfixDelimiter()
            ? new(
                new ParentheticalExpression(predicate.Expression),
                matchResult)
            : throw new UnexpectedTokenException($"unexpected token '{matchResult.Script.ReadSymbol(in matchResult.Symbol)}' at offset {matchResult.Script.Offset}. expected {nameof(Tokens.CloseCircumfixDelimiter)}.");
    }

    private static ComparisonOperator ParseComparisonOperator(
        ref readonly MatchResult matchResult)
    {
        CheckEndOfSource(in matchResult);

        return matchResult.Symbol.IsOperator()
            ? (ComparisonOperator)matchResult.Symbol.TokenId
            : throw new UnexpectedTokenException($"unexpected token '{matchResult.Script.ReadSymbol(in matchResult.Symbol)}' at offset {matchResult.Script.Offset}. expected comparison operator.");
    }

    private static Keyword ParseKeyword(
        ref readonly MatchResult matchResult,
        Keywords expectedWord)
    {
        CheckEndOfSource(in matchResult);

        return matchResult.Symbol.IsKeyword()
            && (Keywords)matchResult.Symbol.TokenId == expectedWord
                ? (Keyword)matchResult.Symbol.TokenId
                : throw new UnexpectedTokenException($"unexpected token '{matchResult.Script.ReadSymbol(in matchResult.Symbol)}' at offset {matchResult.Script.Offset}. expected {nameof(Tokens.Keyword)}.");
    }

    private static Identifier ParseIdentifier(
        ref readonly MatchResult matchResult)
    {
        CheckEndOfSource(in matchResult);

        return matchResult.Symbol.IsIdentifier()
            ? (Identifier)matchResult.Script.ReadSymbol(in matchResult.Symbol)
            : throw new UnexpectedTokenException($"unexpected token '{matchResult.Script.ReadSymbol(in matchResult.Symbol)}' at offset {matchResult.Script.Offset}. expected {nameof(Tokens.Identifier)}.");
    }

    [SuppressMessage("Style", "IDE0010:Add missing cases", Justification = "switch is complete")]
    private static Expression ParseLiteral(
        ref readonly MatchResult matchResult)
    {
        CheckEndOfSource(in matchResult);

        if (matchResult.Symbol.IsLiteral())
        {
            if (matchResult.Symbol.IsNumericLiteral())
            {
                return ParseNumericLiteral(in matchResult);
            }

            switch (matchResult.Symbol.Token)
            {
                case Tokens.StringLiteral:
                    return (StringLiteral)matchResult.Script.ReadSymbol(in matchResult.Symbol);

                case Tokens.CharacterLiteral:
                    return (CharacterLiteral)matchResult.Script.ReadSymbol(in matchResult.Symbol);

                case Tokens.BooleanFalseLiteral:
                    return (BooleanLiteral)false;

                case Tokens.BooleanTrueLiteral:
                    return (BooleanLiteral)true;
            }
        }

        throw new UnexpectedTokenException($"unexpected token '{matchResult.Script.ReadSymbol(in matchResult.Symbol)}' at offset {matchResult.Script.Offset}. expected {nameof(Tokens.Literal)}.");
    }

    [SuppressMessage("Style", "IDE0072:Add missing cases", Justification = "switch is complete")]
    private static NumericLiteral ParseNumericLiteral(
        ref readonly MatchResult matchResult)
    {
        CheckEndOfSource(in matchResult);

        var value = matchResult
            .Script
            .ReadSymbol(in matchResult.Symbol);

        // todo: use TryParse and add error msg on false
        return matchResult.Symbol.Token switch
        {
            Tokens.IntegerLiteral => new NumericLiteral(
                NumericTypes.Integer,
                Int32.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture)),
            Tokens.FloatingPointLiteral => new NumericLiteral(
                NumericTypes.FloatingPoint,
                Double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture)),
            Tokens.ScientificNotationLiteral => new NumericLiteral(
                NumericTypes.ScientificNotation,
                Double.Parse(value, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture)),
            _ => new NumericLiteral(NumericTypes.NotANumber, 0)
        };
    }

    private (NumericLiteral? skip, NumericLiteral? take) ParseSkipTake(
        MatchResult matchResult)
    {
        var skip = default(NumericLiteral?);
        var take = default(NumericLiteral?);

        matchResult = lexer.NextMatch(matchResult);

        if (IsEndOfSource(in matchResult))
        {
            return (skip, take);
        }

        if (matchResult.Symbol.IsKeyword())
        {
            if (matchResult.Symbol.TokenId == TokenIds.SKIP)
            {
                matchResult = lexer.NextMatch(matchResult);
                skip = ParseNumericLiteral(in matchResult);

                matchResult = lexer.NextMatch(matchResult);
            }

            if (matchResult.Symbol.IsKeyword() &&
                matchResult.Symbol.TokenId == TokenIds.TAKE)
            {
                matchResult = lexer.NextMatch(matchResult);
                CheckEndOfSource(in matchResult);
                take = ParseNumericLiteral(in matchResult);
            }

            return (skip, take);
        }

        throw new UnexpectedTokenException($"unexpected token '{matchResult.Script.ReadSymbol(in matchResult.Symbol)}' at offset {matchResult.Script.Offset}. expected ({nameof(Keywords.Skip)} | {nameof(Keywords.Take)}).");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CheckEndOfSource(ref readonly MatchResult matchResult)
    {
        if (IsEndOfSource(in matchResult))
        {
            throw new UnexpectedEndOfSourceException("Unexpected end of source");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsEndOfSource(ref readonly MatchResult matchResult)
    {
        return matchResult.Symbol.IsEndOfSource();
    }
}
