using Luthor;
using Luthor.Tokens;
using Math.Parser.Expressions;
using System.Globalization;

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

        var tokens = new Lexer(tokenizers, source)
            .Tokens()
            .Where(token => !token.IsWhiteSpace());

        if (tokens.Any(t => t.IsError()))
        {
            return new SyntaxTree(
                new Number(NumberTypes.NotANumber, 0),
                tokens
                    .Where(t => t.IsError())
                    .Select(t => t.Symbol)
                    .ToArray());
        }

        if (tokens.First().IsEndOfSource())
        {
            return new SyntaxTree(new Number(NumberTypes.NotANumber, 0), Array.Empty<string>());
        }

        var errors = new List<string>();
        var index = 0;
        var expression = ParseExpression(
            tokens.ToArray().AsSpan(),
            errors,
            ref index);

        return new SyntaxTree(expression, [.. errors]);
    }

    private static Expression ParseExpression(
        ReadOnlySpan<Token> tokens,
        List<string> errors,
        ref int index)
    {
        var token = tokens[index];
        if (token.IsEndOfSource())
        {
            throw new InvalidOperationException("Unexpected end of source");
        }

        var left = ParseTerm(tokens, errors, ref index);

        token = tokens[index];
        while (!token.IsEndOfSource()
            && token.IsOperator()
            && AdditiveOperators.Contains(token.Symbol))
        {
            ++index;
            left = new BinaryOperation(
                left,
                ParseTerm(tokens, errors, ref index),
                AsOperator(token.Symbol));

            token = tokens[index];
        }

        return left;
    }

    private static Expression ParseTerm(
        ReadOnlySpan<Token> tokens,
        List<string> errors,
        ref int index)
    {
        var token = tokens[index];
        if (token.IsEndOfSource())
        {
            throw new InvalidOperationException("Unexpected end of source");
        }

        var left = ParseFactor(tokens, errors, ref index);

        token = tokens[index];
        while (!token.IsEndOfSource()
            && token.IsOperator()
            && MultiplicitiveOperators.Contains(token.Symbol))
        {
            ++index;
            left = new BinaryOperation(
                left,
                ParseFactor(tokens, errors, ref index),
                AsOperator(token.Symbol));

            token = tokens[index];
        }

        return left;
    }

    private static Expression ParseFactor(
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
            return ParseNumber(token);
        }

        if (token.Type == TokenType.OpenCircumfixDelimiter)
        {
            return ParseGroup(tokens, errors, ref index);
        }

        errors.Add($"Unexpected token {token.Symbol}");
        return new Number(NumberTypes.NotANumber, 0);
    }

    private static Number ParseNumber(Token token)
    {
        // todo: use TryParse and add error msg on false
        return token.Symbol.Contains('.')
            ? new Number(
                NumberTypes.Float,
                Double.Parse(token.Symbol, CultureInfo.InvariantCulture))
            : new Number(
                NumberTypes.Integer,
                Int32.Parse(token.Symbol, CultureInfo.InvariantCulture));
    }

    private static Group ParseGroup(
       ReadOnlySpan<Token> tokens,
       List<string> errors,
       ref int index)
    {
        ++index;
        var expression = ParseExpression(tokens, errors, ref index);
        if (tokens[index].Type != TokenType.CloseCircumfixDelimiter)
        {
            errors.Add("Expected close circumfix delimiter");
            return new Group(new Number(NumberTypes.NotANumber, 0));
        }

        ++index;
        return new Group(expression);
    }
}
