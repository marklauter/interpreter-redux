using Luthor.Tokens;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Luthor.Context;

public sealed partial class LinguisticContext
    : IEnumerable<LinguisticExpression>
{
    private readonly ReadOnlyDictionary<TokenType, LinguisticExpression> map;
    private readonly LinguisticExpression[] expressions = null!;

    public LinguisticContext(LanguageSpecification spec)
    {
        ArgumentNullException.ThrowIfNull(spec);

        expressions = new LinguisticExpression[12];
        var (index, reservedWordPattern) = AddReservedWords(spec, expressions);
        (index, var booleanLiteralPattern) = AddBooleanLiterals(spec, expressions, index);
        index = AddIdentifiers(expressions, index, booleanLiteralPattern, reservedWordPattern);
        index = AddNumericLiterals(expressions, index);
        index = AddStringLiterals(expressions, index);
        index = AddCharacterLiterals(expressions, index);
        index = AddNewLine(expressions, index);
        index = AddWhitespace(expressions, index);
        index = AddInfixDelimiters(spec, expressions, index);
        index = AddCircumfixDelimiters(spec, expressions, index);
        index = AddComments(spec, expressions, index);
        index = AddOperators(spec, expressions, index);

        map = expressions[..index]
            .ToDictionary(e => e.Type, e => e)
            .AsReadOnly();

        expressions = [.. map.Values];
    }

    public int Length => expressions.Length;
    public LinguisticExpression this[int i] => expressions[i];
    public LinguisticExpression this[TokenType key] => map[key];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<LinguisticExpression> AsReadOnlySpan()
    {
        return expressions.AsSpan();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<LinguisticExpression> GetEnumerator()
    {
        return (IEnumerator<LinguisticExpression>)expressions.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private const RegexOptions ExpressionOptions =
        RegexOptions.CultureInvariant |
        RegexOptions.ExplicitCapture |
        RegexOptions.Compiled;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int index, string? reservedWordPattern) AddReservedWords(
    LanguageSpecification spec,
    LinguisticExpression[] expressions)
    {
        if (spec.TryGetReservedWordPattern(out var reservedWordPattern))
        {
            expressions[0] = new
            (
                TokenType.ReservedWord,
                new Regex(
                    $@"\G(?:{reservedWordPattern})",
                    ExpressionOptions)
            );

            return (1, reservedWordPattern);
        }

        return (0, reservedWordPattern);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int index, string? booleanLiteralPattern) AddBooleanLiterals(
        LanguageSpecification spec,
        LinguisticExpression[] expressions,
        int index)
    {
        if (spec.TryGetBooleanLiteralPattern(out var booleanLiteralPattern))
        {
            expressions[index] = new
            (
                TokenType.BooleanLiteral,
                new Regex(
                    $@"\G(?:{booleanLiteralPattern})",
                    ExpressionOptions)
            );

            ++index;
        }

        return (index, booleanLiteralPattern);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddIdentifiers(
        LinguisticExpression[] expressions,
        int index,
        string? booleanLiteralPattern,
        string? reservedWordPattern)
    {
        var identifierPattern = reservedWordPattern is null
            ? booleanLiteralPattern is null
                ? $@"\G(?:{RegexConstants.Identifiers})"
                : $@"\G(?:{RegexConstants.Identifiers})(?!{booleanLiteralPattern})"
            : booleanLiteralPattern is null
                ? $@"\G(?:{RegexConstants.Identifiers})(?!{reservedWordPattern})"
                : $@"\G(?:{RegexConstants.Identifiers})(?!{reservedWordPattern}|{booleanLiteralPattern})";

        expressions[index] = new
        (
            TokenType.Identifier,
            new Regex(
                identifierPattern,
                ExpressionOptions)
        );

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddNumericLiterals(
        LinguisticExpression[] expressions,
        int index)
    {
        expressions[index] = new
        (
            TokenType.NumericLiteral,
            NumericLiteralExpression()
        );

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddStringLiterals(
        LinguisticExpression[] expressions,
        int index)
    {
        expressions[index] = new
        (
            TokenType.StringLiteral,
            StringLiteralExpression()
        );

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddCharacterLiterals(
        LinguisticExpression[] expressions,
        int index)
    {
        expressions[index] = new
        (
            TokenType.CharacterLiteral,
            CharacterLiteralExpression()
        );

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddNewLine(
        LinguisticExpression[] expressions,
        int index)
    {
        expressions[index] = new
        (
            TokenType.NewLine,
            NewLineExpression()
        );

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddWhitespace(
        LinguisticExpression[] expressions,
        int index)
    {
        expressions[index] = new
        (
            TokenType.Whitespace,
            WhitespaceExpression()
        );


        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddInfixDelimiters(
        LanguageSpecification spec,
        LinguisticExpression[] expressions,
        int index)
    {
        if (spec.TryGetInfixDelimiterPattern(out var delimiterPattern))
        {
            expressions[index] = new
            (
                TokenType.InfixDelimiter,
                new Regex(
                    $@"\G(?:{delimiterPattern})",
                    ExpressionOptions)
            );

            ++index;
        }

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddCircumfixDelimiters(
        LanguageSpecification spec,
        LinguisticExpression[] expressions,
        int index)
    {
        if (spec.TryGetCircumfixDelimiterOpenPattern(out var delimiterPattern))
        {
            expressions[index] = new
            (
                TokenType.CircumfixDelimiter,
                new Regex(
                    $@"\G(?:{delimiterPattern})",
                    ExpressionOptions)
            );

            ++index;
        }

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddOperators(
        LanguageSpecification spec,
        LinguisticExpression[] expressions,
        int index)
    {
        if (spec.TryGetOperatorPattern(out var operatorPattern))
        {
            expressions[index] = new
            (
                TokenType.Operator,
                new Regex(
                    $@"\G(?:{operatorPattern})",
                    ExpressionOptions)
            );

            ++index;
        }

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddComments(
        LanguageSpecification spec,
        LinguisticExpression[] expressions,
        int index)
    {
        if (spec.TryGetCommentPattern(out var commentPattern))
        {
            expressions[index] = new
            (
                TokenType.Comment,
                new Regex(
                    $@"\G(?:{commentPattern})",
                    ExpressionOptions)
            );

            ++index;
        }

        return index;
    }

    [GeneratedRegex(@"\G(?:\b\d+(?:\.\d+)?\b)", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex NumericLiteralExpression();

    [GeneratedRegex(@"\G(?:""(?:[^""\\\n\r]|\\.)*"")", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex StringLiteralExpression();

    // todo: need to add char literal pattern for escape codes like \b, \t, \n, \r, \f, \', \", \\, \u0000, \uFFFF
    [GeneratedRegex(@"\G(?:'[^']')", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex CharacterLiteralExpression();

    [GeneratedRegex(@"\G(?:\r\n|[\r\n])", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex NewLineExpression();

    [GeneratedRegex(@"\G(?:\s+)", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex WhitespaceExpression();
}
