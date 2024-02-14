using Luthor;
using Luthor.Tokens;
using Predicate.Parser.Expressions;
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

    private static readonly string[] EndOfSourceError = ["unexpected end of source"];
    private static readonly ErrorExpression EndOfSourceErrorExpression = new("unexpected end of source");

    public Expressions.Predicate Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var lexer = new Lexer(tokenizers, source);
        var tokens = lexer
            .Tokens()
            .Where(token => !token.IsWhiteSpace());

        if (tokens.Any(t => t.IsError()))
        {
            return new Expressions.Predicate(
                new ErrorExpression("errors encountered"),
                tokens
                    .Where(t => t.IsError())
                    .Select(lexer.ReadSymbol)
                    .ToArray());
        }

        if (tokens.First().IsEndOfSource())
        {
            return new Expressions.Predicate(
                EndOfSourceErrorExpression,
                EndOfSourceError);
        }

        var errors = new List<string>();
        var index = 0;
        var expression = ParseStatement(
            lexer,
            tokens.ToArray().AsSpan(),
            errors,
            ref index);

        return new Expressions.Predicate(expression, errors);
    }

    private static Expression ParseStatement(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        List<string> errors,
        ref int index)
    {
        var token = tokens[index];
        var expression = ParseReservedWord(
            lexer,
            token,
            ReservedWords.From,
            errors,
            ref index);

        if (expression is ErrorExpression)
        {
            return expression;
        }

        token = tokens[index];
        expression = ParseIdentifier(
            lexer,
            token,
            errors,
            ref index);

        if (expression is Identifier identifier)
        {
            var where = ParseWhereClause(
                lexer,
                tokens,
                errors,
                ref index);

            return new From(
                identifier.Value,
                where);
        }

        return expression;
    }

    private static Expression ParseWhereClause(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        List<string> errors,
        ref int index)
    {
        var token = tokens[index];
        var expression = ParseReservedWord(
            lexer,
            token,
            ReservedWords.Where,
            errors,
            ref index);

        return expression is ErrorExpression
            ? expression
            : ParseLogicalOr(
                lexer,
                tokens,
                errors,
                ref index);
    }

    private static Expression ParseLogicalOr(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        List<string> errors,
        ref int index)
    {
        var left = ParseLogicalAnd(
            lexer,
            tokens,
            errors,
            ref index);

        var token = tokens[index];
        while (
            !token.IsEndOfSource()
            && token.IsOperator()
            && (LogicalOperator)lexer.ReadSymbol(token) == LogicalOperators.Or)
        {
            ++index;
            var right = ParseLogicalAnd(
                lexer,
                tokens,
                errors,
                ref index);

            left = new LogicalExpression(
                left,
                LogicalOperators.Or,
                right);

            token = tokens[index];
        }

        return left;
    }

    private static Expression ParseLogicalAnd(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        List<string> errors,
        ref int index)
    {
        var left = ParseComparison(
            lexer,
            tokens,
            errors,
            ref index);

        var token = tokens[index];
        while (
            !token.IsEndOfSource()
            && token.IsOperator()
            && (LogicalOperator)lexer.ReadSymbol(token) == LogicalOperators.And)
        {
            ++index;
            var right = ParseComparison(
                lexer,
                tokens,
                errors,
                ref index);

            left = new LogicalExpression(
                left,
                LogicalOperators.Or,
                right);

            token = tokens[index];
        }

        return left;
    }

    private static Expression ParseComparison(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        List<string> errors,
        ref int index)
    {
        var token = tokens[index];
        if (token.IsIdentifier())
        {
            var identifier = (Identifier)lexer.ReadSymbol(token);
            ++index;
            token = tokens[index];
            if (token.IsOperator())
            {
                ++index;
                var value = ParseValue(
                    lexer,
                    tokens,
                    errors,
                    ref index);

                return new ComparisonExpression(
                    identifier,
                    (ComparisonOperator)lexer.ReadSymbol(token),
                    value);
            }
        }

        if (token.IsOpenCircumfixDelimiter())
        {
            return ParseParentheticalGroup(
                lexer,
                tokens,
                errors,
                ref index);
        }

        var message = $"unexpected token {lexer.ReadSymbol(token)}. expected identifier or open parenthesis.";
        errors.Add(message);
        return new ErrorExpression(message);

    }

    private static Expression ParseParentheticalGroup(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        List<string> errors,
        ref int index)
    {
        throw new NotImplementedException(nameof(ParseParentheticalGroup));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CheckEndOfSource(Token token)
    {
        if (token.IsEndOfSource())
        {
            throw new InvalidOperationException("Unexpected end of source");
        }
    }

    private static Expression ParseReservedWord(
        Lexer lexer,
        Token token,
        ReservedWords expectedWord,
        List<string> errors,
        ref int index)
    {
        CheckEndOfSource(token);

        if (!token.IsReservedWord())
        {
            var message = $"unexpected token {lexer.ReadSymbol(token)}. expected reserved word '{expectedWord}'.";
            errors.Add(message);
            return new ErrorExpression(message);
        }

        var reservedWord = (ReservedWord)lexer.ReadSymbol(token);
        if (reservedWord.Value != expectedWord)
        {
            var message = $"unexpected token {lexer.ReadSymbol(token)}. expected reserved word '{expectedWord}'.";
            errors.Add(message);
            return new ErrorExpression(message);
        }

        ++index;
        return new ReservedWordExpression(reservedWord);
    }

    private static Expression ParseIdentifier(
        Lexer lexer,
        Token token,
        List<string> errors,
        ref int index)
    {
        CheckEndOfSource(token);

        if (!token.IsIdentifier())
        {
            var message = $"unexpected token {lexer.ReadSymbol(token)}. expected identifier.";
            errors.Add(message);
            return new ErrorExpression(message);
        }

        ++index;
        return (Identifier)lexer.ReadSymbol(token);
    }

    private static Expression ParseValue(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        List<string> errors,
        ref int index)
    {
        var token = tokens[index];
        CheckEndOfSource(token);

        if (token.IsLiteral())
        {
            var symbol = lexer.ReadSymbol(token);
            if (token.IsNumber())
            {
                ++index;
                return (NumericLiteral)symbol;
            }

            if (token.IsString())
            {
                ++index;
                return (StringLiteral)symbol;
            }

            if (token.IsBoolean())
            {
                ++index;
                return (BooleanLiteral)symbol;
            }
        }

        ++index;
        var message = $"unexpected token {lexer.ReadSymbol(token)}. expected literal.";
        errors.Add(message);
        return new ErrorExpression(message);
    }
}
