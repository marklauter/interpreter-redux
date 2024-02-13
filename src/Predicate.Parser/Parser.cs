﻿using Luthor;
using Luthor.Tokens;
using Predicate.Parser.Expressions;

namespace Predicate.Parser;

/*
https://bnfplayground.pauliankline.com/?bnf=%3Cpredicate%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cfromclause%3E%20%3Crequiredwhitespace%3E%20%3Cwhereclause%3E%0A%3Cfromclause%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%22from%22%20%3Crequiredwhitespace%3E%20%3Cidentifier%3E%0A%3Cwhereclause%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%22where%22%20%3Crequiredwhitespace%3E%20%3Ccondition%3E%0A%3Ccondition%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cterm%3E%20%7C%20%3Ccondition%3E%20%3Cor%3E%20%3Cterm%3E%0A%3Cor%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Crequiredwhitespace%3E%20%22%7C%7C%22%20%3Crequiredwhitespace%3E%20%0A%3Cterm%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cfactor%3E%20%7C%20%3Cterm%3E%20%3Cand%3E%20%3Cfactor%3E%0A%3Cand%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Crequiredwhitespace%3E%20%22%26%26%22%20%3Crequiredwhitespace%3E%20%0A%3Cfactor%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cproperty%3E%20%3Ccomparisonoperator%3E%20%3Cvalue%3E%20%7C%20%3Cparentheticalexpression%3E%0A%3Cparentheticalexpression%3E%20%20%20%3A%3A%3D%20%22(%22%20%3Coptionalwhitespace%3E%20%3Ccondition%3E%20%3Coptionalwhitespace%3E%20%22)%22%0A%3Crequiredwhitespace%3E%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cwhitespace%3E%2B%0A%3Coptionalwhitespace%3E%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cwhitespace%3E*%0A%3Cproperty%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cidentifier%3E%0A%3Ccomparisonoperator%3E%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Coptionalwhitespace%3E%20(%22%3D%3D%22%20%7C%20%22!%3D%22%20%7C%20%22%3C%22%20%7C%20%22%3E%22%20%7C%20%22%3C%3D%22%20%7C%20%22%3E%3D%22%20%7C%20%22-s%22%20%7C%20%22-e%22%20%7C%20%22-c%22)%20%3Coptionalwhitespace%3E%0A%3Cvalue%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cstringliteral%3E%20%7C%20%3Cnumericliteral%3E%0A%3Cidentifier%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20(%3Cuppercase%3E%20%7C%20%3Clowercase%3E)%2B%20%3Ccharacter%3E*%0A%3Cstringliteral%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cquote%3E%20%3Cstring%3E%20%3Cquote%3E%0A%3Cstring%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20(%3Ccharacter%3E%20%7C%20%3Cwhitespace%3E)*%0A%3Cquote%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%22%5C%22%22%0A%3Cnumericliteral%3E%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cpositivenumber%3E%20%7C%20%3Cnegativenumber%3E%0A%3Cnegativenumber%3E%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%22-%22%20%3Cpositivenumber%3E%0A%3Cpositivenumber%3E%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20(%220%22%20%7C%20%20%5B1-9%5D%20%3Cdigit%3E*)%20(%22.%22%20%3Cdigit%3E%2B%20)%3F%0A%3Cinteger%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cdigit%3E%2B%0A%3Ccharacter%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%3Cuppercase%3E%20%7C%20%3Clowercase%3E%20%7C%20%3Cdigit%3E%0A%3Cuppercase%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%5BA-Z%5D%0A%3Clowercase%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%5Ba-z%5D%0A%3Cdigit%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20%5B0-9%5D%0A%3Cwhitespace%3E%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3A%3A%3D%20(%22%20%22%20%7C%20%22%5Ct%22%20%7C%20%22%5Cr%22%20%7C%20%22%5Cn%22%20%7C%20%22%5Cr%5Cn%22)&name=predicate%20expression

<predicate>                 ::= <fromclause> <requiredwhitespace> <whereclause>
<fromclause>                ::= "from" <requiredwhitespace> <identifier>
<whereclause>               ::= "where" <requiredwhitespace> <condition>
<condition>                 ::= <term> | <condition> <or> <term>
<or>                        ::= <requiredwhitespace> "||" <requiredwhitespace> 
<term>                      ::= <factor> | <term> <and> <factor>
<and>                       ::= <requiredwhitespace> "&&" <requiredwhitespace> 
<factor>                    ::= <property> <comparisonoperator> <value> | <parentheticalexpression>
<parentheticalexpression>   ::= "(" <optionalwhitespace> <condition> <optionalwhitespace> ")"
<requiredwhitespace>        ::= <whitespace>+
<optionalwhitespace>        ::= <whitespace>*
<property>                  ::= <identifier>
<comparisonoperator>        ::= <optionalwhitespace> ("==" | "!=" | "<" | ">" | "<=" | ">=" | "-s" | "-e" | "-c") <optionalwhitespace>
<value>                     ::= <stringliteral> | <numericliteral>
<identifier>                ::= (<uppercase> | <lowercase>)+ <character>*
<stringliteral>             ::= <quote> <string> <quote>
<string>                    ::= (<character> | <whitespace>)*
<quote>                     ::= "\""
<numericliteral>            ::= <positivenumber> | <negativenumber>
<negativenumber>            ::= "-" <positivenumber>
<positivenumber>            ::= ("0" |  [1-9] <digit>*) ("." <digit>+ )?
<integer>                   ::= <digit>+
<character>                 ::= <uppercase> | <lowercase> | <digit>
<uppercase>                 ::= [A-Z]
<lowercase>                 ::= [a-z]
<digit>                     ::= [0-9]
<whitespace>                ::= (" " | "\t" | "\r" | "\n" | "\r\n")

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
        var expression = ParsePredicate(
            lexer,
            tokens.ToArray().AsSpan(),
            errors,
            ref index);

        return new Expressions.Predicate(expression, errors);
    }

    private static Expression ParsePredicate(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        List<string> errors,
        ref int index)
    {
        return ParseFromClause(
            lexer,
            tokens,
            errors,
            ref index);
    }

    private static Expression ParseFromClause(
        Lexer lexer,
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
            lexer,
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
            lexer,
            token,
            errors,
            ref index);

        return expression is Identfier identifier
            ? new From(
                identifier.Value,
                new ErrorExpression("not implemented"))
            : expression;
    }

    private static Expression ParseReservedWord(
        Lexer lexer,
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
        if (token.IsEndOfSource())
        {
            throw new InvalidOperationException("Unexpected end of source");
        }

        if (!token.IsIdentifier())
        {
            var message = $"unexpected token {lexer.ReadSymbol(token)}. expected identifier.";
            errors.Add(message);
            return new ErrorExpression(message);
        }

        ++index;
        return new Identfier(lexer.ReadSymbol(token));
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
