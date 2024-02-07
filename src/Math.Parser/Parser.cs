using Luthor;
using Luthor.Tokens;
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

    private static Operators AsOperator(string symbol)
    {
        return symbol switch
        {
            "+" => Operators.Add,
            "-" => Operators.Subtract,
            "*" => Operators.Multiply,
            "/" => Operators.Divide,
            "%" => Operators.Modulus,
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
        var expression = ParseExpression(
            tokens.ToArray().AsSpan(),
            errors);

        return new SyntaxTree(expression, [.. errors]);
    }

    private static Expression ParseExpression(
        ReadOnlySpan<Token> tokens,
        List<string> errors)
    {
        var index = 0;
        return ParseTerm(tokens, errors, ref index);
    }

    private static Expression ParseTerm(
        ReadOnlySpan<Token> tokens,
        List<string> errors,
        ref int index)
    {
        var left = ParseFactor(tokens, errors, ref index);

        var token = tokens[index];
        while (!token.IsEndOfSource()
            && token.IsOperator()
            && AdditiveOperators.Contains(token.Symbol))
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

        var left = ParseNumber(token, errors);

        token = tokens[++index];
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

    private static Expression ParseNumber(
        Token token,
        List<string> errors)
    {
        if (!token.IsNumber())
        {
            errors.Add($"Unexpected token {token.Symbol}");
            return new Number(NumberTypes.NotANumber, 0);
        }

        return token.Symbol.Contains('.')
            ? new Number(
                NumberTypes.Float,
                Double.Parse(token.Symbol, CultureInfo.InvariantCulture))
            : new Number(
                NumberTypes.Integer,
                Int32.Parse(token.Symbol, CultureInfo.InvariantCulture));
    }

    //else if (token.Type == TokenType.OpenCircumfixDelimiter)
    //{
    //    // todo: group expression
    //}

    //    if (match.Token.Type == TokenKind.OpenCircumfixDelimiter)
    //    {
    //        lexer.NextToken(source, ref match);
    //        var expression = ParseExpression(source, lexer, ref match);
    //        if (match.Token.Type != TokenKind.CloseCircumfixDelimiter)
    //        {
    //            throw new InvalidOperationException("Expected closing parenthesis");
    //        }

    //        lexer.NextToken(source, ref match);
    //        return expression;
    //    }
    //    else
    //    {
    //        throw new InvalidOperationException($"Unexpected token {match.Token.Symbol}");
    //    }

    //private static Expression ParseMultiplyDivide(Lexer lexer)
    //{
    //    var left = ParseFactor(
    //        source,
    //        lexer,
    //        ref match);

    //    while (match.Token.Type == TokenKind.Operator
    //        && MultiplicitiveOperators.Contains(match.Token.Symbol))
    //    {
    //        var @operator = ReadOperator(match.Token.Symbol);

    //        var right = ParseFactor(
    //            source,
    //            lexer,
    //            ref match);

    //        left = new BinaryOperation(
    //            left,
    //            right,
    //            @operator);
    //    }

    //    return left;
    //}

    //private static bool TryParseGroup(ref MatchResult match, out Group? group)
    //{
    //    if (match.Token.Type == TokenKind.OpenCircumfixDelimiter)
    //    {

    //    }
    //}

    /*
    private ExpressionNode ParseFactor()
        {
            if (position >= input.Length)
                throw new ArgumentException("Unexpected end of input");

            if (char.IsDigit(input[position]))
            {
                // Parse a number
                int start = position;
                while (position < input.Length && char.IsDigit(input[position]))
                    position++;

                int value = int.Parse(input[start..position]);
                return new NumberNode { Value = value };
            }
            else if (input[position] == '(')
            {
                // Parse a parenthesized expression
                position++; // Skip the opening parenthesis
                var innerExpression = ParseExpression();
                if (position >= input.Length || input[position] != ')')
                    throw new ArgumentException("Missing closing parenthesis");
                position++; // Skip the closing parenthesis
                return new ParenthesizedExpressionNode { InnerExpression = innerExpression };
            }
            else
            {
                throw new ArgumentException($"Unexpected character '{input[position]}' at position {position}");
            }
        }

     */
}
