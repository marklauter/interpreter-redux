using Luthor;
using Luthor.Tokens;
using Predicate.Parser.Expressions;

namespace Predicate.Parser;

/*
<predicate>             ::= <from-clause> <where-clause>
<from-clause>           ::= "from" <identifier>
<where-clause>          ::= "where" <condition>
<condition>             ::= <expression> | <grouped-expression>
<grouped-expression>    ::= "(" <expression> ")"
<expression>            ::= <term> | <expression> "||" <term>
<term>                  ::= <factor> | <term> "&&" <factor>
<factor>                ::= <property> <comparison> <value>
<property>              ::= <identifier>
<comparison>            ::= "==" | "!=" | "<" | ">" | "<=" | ">=" | "-s" | "-e" | "-c"
<value>                 ::= <string-literal> | <numeric-literal>
<string-literal>        ::= <quoted-string>
<numeric-literal>       ::= <number>

bottom up:
parse numeric literal
parse string literal
parse value
parse property
parse factor
parse term
parse expression
parse grouped expression
parse condition
parse predicate
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

        var tokens = new Lexer(tokenizers, source)
            .Tokens()
            .Where(token => !token.IsWhiteSpace());

        if (tokens.Any(t => t.IsError()))
        {
            return new Expressions.Predicate(
                new ErrorExpression("errors encountered"),
                tokens
                    .Where(t => t.IsError())
                    .Select(t => t.Symbol)
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
        var expression = ParsePredicate(
            tokens.ToArray().AsSpan(),
            errors,
            ref index);

        return new Expressions.Predicate(expression, errors);
    }

    private static Expression ParsePredicate(
        ReadOnlySpan<Token> tokens,
        List<string> errors,
        ref int index)
    {
        return ParseFromClause(tokens, errors, ref index);
    }

    private static Expression ParseFromClause(
        ReadOnlySpan<Token> tokens,
        List<string> errors,
        ref int index)
    {
        var token = tokens[index];
        if (token.IsEndOfSource())
        {
            throw new InvalidOperationException("Unexpected end of source");
        }

        var expression = ParseReservedWord(
            token,
            ReservedWords.From,
            errors,
            ref index);

        if (expression is not ReservedWordExpression)
        {
            return expression;
        }

        token = tokens[index];
        expression = ParseIdentifier(
            token,
            errors,
            ref index);

        return expression is Identfier identifier
            ? new FromClause(
                identifier.Value,
                new ErrorExpression("not implemented"))
            : expression;
    }

    private static Expression ParseReservedWord(
        Token token,
        ReservedWords expectedWord,
        List<string> errors,
        ref int index)
    {
        if (token.IsEndOfSource())
        {
            throw new InvalidOperationException("Unexpected end of source");
        }

        if (!token.IsReservedWord())
        {
            var message = $"unexpected token {token.Symbol}. expected reserved word '{expectedWord}'.";
            errors.Add(message);
            return new ErrorExpression(message);
        }

        var reservedWord = (ReservedWord)token.Symbol;
        if (reservedWord.Value != expectedWord)
        {
            var message = $"unexpected token {token.Symbol}. expected reserved word '{expectedWord}'.";
            errors.Add(message);
            return new ErrorExpression(message);
        }

        ++index;
        return new ReservedWordExpression(reservedWord);
    }

    private static Expression ParseIdentifier(
        Token token,
        List<string> errors,
        ref int index)
    {
        if (token.IsEndOfSource())
        {
            throw new InvalidOperationException("Unexpected end of source");
        }

        if (!token.IsIdentifier())
        {
            var message = $"unexpected token {token.Symbol}. expected identifier.";
            errors.Add(message);
            return new ErrorExpression(message);
        }

        ++index;
        return new Identfier(token.Symbol);
    }

    //private static Expression ParseLiteral(
    //    ReadOnlySpan<Token> tokens,
    //    List<string> errors,
    //    ref int index)
    //{
    //    var token = tokens[index];
    //    if (token.IsEndOfSource())
    //    {
    //        throw new InvalidOperationException("Unexpected end of source");
    //    }

    //    if (!token.IsLiteral())
    //    {
    //        ++index;
    //        errors.Add($"Unexpected token {token.Symbol}. Expected literal.");
    //    }

    //    if (token.IsNumber())
    //    {
    //        ++index;
    //        return ParseNumericLiteral(token);
    //    }

    //    if (token.IsString())
    //    {
    //        ++index;
    //        return ParseStringLiteral(token);
    //    }

    //    if (token.IsBoolean())
    //    {
    //        ++index;
    //        return ParseStringLiteral(token);
    //    }
    //}
}
