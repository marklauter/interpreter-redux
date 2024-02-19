using Lexi;
using Math.Parser.Expressions;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Math.Parser;

public sealed class Parser(Lexer lexer)
{
    private readonly Lexer lexer = lexer
        ?? throw new ArgumentNullException(nameof(lexer));

    private readonly ref struct ParseResult(
        Expression expression,
        MatchResult matchResult)
    {
        public readonly Expression Expression = expression;
        public readonly MatchResult MatchResult = matchResult;
    }

    public Expression Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var script = new Script(source);
        var parseResult = ParseTerm(script);
        return parseResult.Expression;
    }

    private ParseResult ParseTerm(Script script)
    {
        var left = ParseFactor(script);

        var matchResult = left.MatchResult;
        matchResult = lexer.NextMatch(matchResult.Script);

        while (!matchResult.Script.IsEndOfSource()
            && matchResult.Symbol.IsOperator()
            && matchResult.Symbol.TokenId.IsTerm())
        {
            var right = ParseFactor(matchResult.Script);

            left = new ParseResult(
                new BinaryOperation(
                left.Expression,
                right.Expression,
                matchResult.Symbol.TokenId),
                right.MatchResult);

            matchResult = lexer.NextMatch(right.MatchResult.Script);
        }

        return left;
    }

    private ParseResult ParseFactor(Script script)
    {
        var left = ParseValue(script);

        var matchResult = left.MatchResult;
        matchResult = lexer.NextMatch(matchResult.Script);

        while (!matchResult.Script.IsEndOfSource()
            && matchResult.Symbol.IsOperator()
            && matchResult.Symbol.TokenId.IsFactor())
        {
            var right = ParseValue(matchResult.Script);

            left = new ParseResult(
                new BinaryOperation(
                left.Expression,
                right.Expression,
                matchResult.Symbol.TokenId),
                right.MatchResult);

            matchResult = lexer.NextMatch(right.MatchResult.Script);
        }

        return left;
    }

    private ParseResult ParseValue(Script script)
    {
        if (script.IsEndOfSource())
        {
            throw new UnexpectedEndOfSourceException("Unexpected end of source");
        }

        string value;
        var matchResult = lexer.NextMatch(script);

        if (matchResult.Symbol.IsNumericLiteral())
        {
            return new(ParseNumber(in matchResult), matchResult);
        }
        else if (matchResult.Symbol.IsOpenCircumfixDelimiter())
        {
            var term = ParseTerm(matchResult.Script);
            matchResult = lexer.NextMatch(term.MatchResult.Script);
            if (matchResult.Symbol.IsCloseCircumfixDelimiter())
            {
                return new(term.Expression, matchResult);
            }

            value = matchResult.Symbol.Token == Tokens.Undefined
                ? script.Source[script.Offset..]
                : matchResult
                    .Script
                    .ReadSymbol(in matchResult.Symbol);
            throw new UnexpectedTokenException($"unexpected token '{value}' at {matchResult.Symbol.Offset}. expected close parenthesis.");
        }

        value = matchResult.Symbol.Token == Tokens.Undefined
            ? script.Source[script.Offset..]
            : matchResult
                .Script
                .ReadSymbol(in matchResult.Symbol);
        throw new UnexpectedTokenException($"unexpected token '{value}' at {matchResult.Symbol.Offset}. expected number or open parenthesis.");
    }

    [SuppressMessage("Style", "IDE0072:Add missing cases", Justification = "switch is complete")]
    private static Number ParseNumber(ref readonly MatchResult matchResult)
    {
        var value = matchResult
            .Script
            .ReadSymbol(in matchResult.Symbol);

        // todo: use TryParse and add error msg on false
        return matchResult.Symbol.Token switch
        {
            Tokens.IntegerLiteral => new Number(
                NumericTypes.Integer,
                Int32.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture)),
            Tokens.FloatingPointLiteral => new Number(
                NumericTypes.FloatingPoint,
                Double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture)),
            Tokens.ScientificNotationLiteral => new Number(
                NumericTypes.ScientificNotation,
                Double.Parse(value, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture)),
            _ => new Number(NumericTypes.NotANumber, 0)
        };
    }
}
