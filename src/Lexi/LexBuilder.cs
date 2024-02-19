using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Lexi;

public class LexiPatternBuilder
{
    private readonly List<TokenPattern> patterns = [];

    private LexiPatternBuilder() { }

    private LexiPatternBuilder(TokenPattern[] patterns)
    {
        ArgumentNullException.ThrowIfNull(patterns);
        this.patterns.AddRange(patterns);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LexiPatternBuilder Create()
    {
        return new LexiPatternBuilder();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LexiPatternBuilder Create(TokenPattern[] patterns)
    {
        return new LexiPatternBuilder(patterns);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Lexer Build()
    {
        return new Lexer([.. patterns]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddCommentPrefix(
        string pattern,
        int id)
    {
        return AddPattern(
            pattern,
            Tokens.Comment,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddClosingCircumfixDelimiter(
        string pattern,
        int id)
    {
        return AddPattern(
            pattern,
            Tokens.CloseCircumfixDelimiter,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddOpeningCircumfixDelimiter(
        string pattern,
        int id)
    {
        return AddPattern(
            pattern,
            Tokens.OpenCircumfixDelimiter,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddInfixDelimiter(
        string pattern,
        int id)
    {
        return AddPattern(
            pattern,
            Tokens.InfixDelimiter,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddDelimiter(
        string pattern,
        int id)
    {
        return AddPattern(
            pattern,
            Tokens.Delimiter,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddOperator(
        string pattern,
        int id)
    {
        return AddPattern(
            pattern,
            Tokens.Operator,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddBooleanFalseLiteral(
        string pattern,
        int id)
    {
        return AddPattern(
            pattern,
            Tokens.BooleanFalseLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddBooleanTrueLiteral(
        string pattern,
        int id)
    {
        return AddPattern(
            pattern,
            Tokens.BooleanTrueLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddBooleanLiteral(
        string pattern,
        int id)
    {
        return AddPattern(
            pattern,
            Tokens.BooleanLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddScientificNotationLiteral(
        int id)
    {
        return AddPattern(
            AuxillaryPatterns.ScientificNotationLiteral(),
            Tokens.ScientificNotationLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddFloatingPointLiteral(
        int id)
    {
        return AddPattern(
            AuxillaryPatterns.FloatingPointLiteral(),
            Tokens.FloatingPointLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddIntegerLiteral(
        int id)
    {
        return AddPattern(
            AuxillaryPatterns.IntegerLiteral(),
            Tokens.IntegerLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddStringLiteral(
        int id)
    {
        return AddPattern(
            AuxillaryPatterns.QuotedStringLiteral(),
            Tokens.StringLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddCharacterLiteral(
        int id)
    {
        return AddPattern(
            AuxillaryPatterns.CharacterLiteral(),
            Tokens.CharacterLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddNulliteral(
        string pattern,
        int id)
    {
        return AddPattern(
            pattern,
            Tokens.NullLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddLiteral(
        string pattern,
        int id)
    {
        return AddPattern(pattern, Tokens.Literal, id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddKeyword(
        string pattern,
        int id)
    {
        return AddPattern(pattern, Tokens.Keyword, id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddIdentifier(
        int id)
    {
        return AddPattern(
            AuxillaryPatterns.Identifier(),
            Tokens.Identifier,
            id);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddPattern(
        string pattern,
        Tokens tokenClass,
        int id)
    {
        patterns.Add(new(
            @$"\G{pattern}",
            tokenClass,
            id));
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexiPatternBuilder AddPattern(
        Regex regex,
        Tokens tokenClass,
        int id)
    {
        patterns.Add(new(
            regex,
            tokenClass,
            id));
        return this;
    }
}
