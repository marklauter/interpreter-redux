﻿using Luthor;
using Luthor.Tokens;
using Math.Parser.Expressions;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Math.Parser;

/*
<expression> ::= <term> | <expression> "+" <term> | <expression> "-" <term>
<term>       ::= <factor> | <term> "*" <factor> | <term> "/" <factor>
<factor>     ::= <number> | "(" <expression> ")"
<number>     ::= <digit> | <digit> <number>
<digit>      ::= "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"

bottom up:
parse number
parse factor
parse term
parse expression
*/

public sealed class Parser(Tokenizers tokenizers)
{
    private readonly Tokenizers tokenizers = tokenizers
        ?? throw new ArgumentNullException(nameof(tokenizers));
    private static readonly HashSet<string> AdditiveOperators = ["+", "-"];
    private static readonly HashSet<string> MultiplicitiveOperators = ["*", "/", "%"];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static OperatorTypes AsOperator(string symbol)
    {
        return symbol switch
        {
            "+" => OperatorTypes.Add,
            "-" => OperatorTypes.Subtract,
            "*" => OperatorTypes.Multiply,
            "/" => OperatorTypes.Divide,
            "%" => OperatorTypes.Modulus,
            _ => throw new InvalidOperationException(),
        };
    }

    public SyntaxTree Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var lexer = new Lexer(tokenizers, source);
        var tokens = lexer
            .Tokens()
            .Where(token => !token.IsWhiteSpace());

        if (tokens.Any(t => t.IsError()))
        {
            return new SyntaxTree(
                new Number(NumericTypes.NotANumber, 0),
                tokens
                    .Where(t => t.IsError())
                    .Select(lexer.ReadSymbol)
                    .ToArray());
        }

        if (tokens.First().IsEndOfSource())
        {
            return new SyntaxTree(new Number(NumericTypes.NotANumber, 0), []);
        }

        var errors = new List<string>();
        var index = 0;
        var expression = ParseExpression(
            lexer,
            tokens.ToArray().AsSpan(),
            errors,
            ref index);

        return new SyntaxTree(expression, [.. errors]);
    }

    private static Expression ParseExpression(
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

        var left = ParseTerm(
            lexer,
            tokens,
            errors,
            ref index);

        token = tokens[index];
        while (
            !token.IsEndOfSource()
            && token.IsOperator()
            && AdditiveOperators.Contains(lexer.ReadSymbol(token)))
        {
            ++index;
            var right = ParseTerm(
                lexer,
                tokens,
                errors,
                ref index);

            left = new BinaryOperation(
                left,
                right,
                AsOperator(lexer.ReadSymbol(token)));

            token = tokens[index];
        }

        return left;
    }

    private static Expression ParseTerm(
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

        var left = ParseFactor(
            lexer,
            tokens,
            errors,
            ref index);

        token = tokens[index];
        while (!token.IsEndOfSource()
            && token.IsOperator()
            && MultiplicitiveOperators.Contains(lexer.ReadSymbol(token)))
        {
            ++index;
            var right = ParseFactor(
                lexer,
                tokens,
                errors,
                ref index);

            left = new BinaryOperation(
                left,
                right,
                AsOperator(lexer.ReadSymbol(token)));

            token = tokens[index];
        }

        return left;
    }

    private static Expression ParseFactor(
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

        if (token.IsNumber())
        {
            ++index;
            return ParseNumber(lexer.ReadSymbol(token));
        }

        if (token.Type == TokenType.OpenCircumfixDelimiter)
        {
            return ParseGroup(
                lexer,
                tokens,
                errors,
                ref index);
        }

        errors.Add($"Unexpected token {lexer.ReadSymbol(token)}");
        return new Number(NumericTypes.NotANumber, 0);
    }

    private static Number ParseNumber(string symbol)
    {
        // todo: use TryParse and add error msg on false
        return symbol.Contains('.')
            ? new Number(
                NumericTypes.FloatingPoint,
                Double.Parse(symbol, CultureInfo.InvariantCulture))
            : new Number(
                NumericTypes.Integer,
                Int32.Parse(symbol, CultureInfo.InvariantCulture));
    }

    private static Group ParseGroup(
        Lexer lexer,
        ReadOnlySpan<Token> tokens,
        List<string> errors,
        ref int index)
    {
        ++index;
        var expression = ParseExpression(
            lexer,
            tokens,
            errors,
            ref index);

        if (tokens[index].Type != TokenType.CloseCircumfixDelimiter)
        {
            errors.Add("Expected close circumfix delimiter");
            return new Group(new Number(NumericTypes.NotANumber, 0));
        }

        ++index;
        return new Group(expression);
    }
}
