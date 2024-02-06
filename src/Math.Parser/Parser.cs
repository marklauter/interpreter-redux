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
*/

public sealed class Parser(Tokenizers tokenizers)
{
    private readonly Tokenizers tokenizers = tokenizers
        ?? throw new ArgumentNullException(nameof(tokenizers));

    public Expression Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        ReadOnlySpan<Token> tokens = new Lexer(tokenizers, source)
            .Tokens()
            .Where(token => !token.IsWhiteSpace())
            .ToArray()
            .AsSpan();

        return ParseExpression(tokens);
    }

    private static Expression ParseExpression(ReadOnlySpan<Token> tokens)
    {
        var index = 0;
        return ParseFactor(tokens, ref index);
    }

    private static readonly HashSet<string> AdditiveOperators = ["+", "-"];
    private static readonly HashSet<string> MultiplicitiveOperators = ["*", "/", "%"];

    private static Operators ReadOperator(string symbol)
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

    private static bool TryParseNumber(
        Token token,
        out Number number)
    {
        var isNumber = token.IsNumber();
        number = isNumber
            ? token.Symbol.Contains('.')
                ? new Number(
                    NumberTypes.Float,
                    Double.Parse(token.Symbol, CultureInfo.InvariantCulture))
                : new Number(
                    NumberTypes.Integer,
                    Int32.Parse(token.Symbol, CultureInfo.InvariantCulture))
            : new Number(NumberTypes.NotANumber, 0);

        return isNumber;
    }

    private static Expression ParseFactor(ReadOnlySpan<Token> tokens, ref int index)
    {
        var token = tokens[index];
        if (token.IsEndOfSource())
        {
            throw new InvalidOperationException("Unexpected end of source");
        }

        if (TryParseNumber(token, out var number))
        {
            ++index;
            return number;
        }

        throw new InvalidOperationException($"Unexpected token {token.Symbol}");
    }

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
