using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Lexi;

public class LexerBuilder
{
    private readonly List<TokenPattern> patterns = [];

    private RegexOptions regexOptions =
        RegexOptions.ExplicitCapture |
        RegexOptions.Compiled |
        RegexOptions.Singleline;

    private LexerBuilder() { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LexerBuilder Create()
    {
        return new LexerBuilder();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LexerBuilder Create(RegexOptions regexOptions)
    {
        var builder = new LexerBuilder();
        builder.regexOptions |= regexOptions;
        return builder;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder WithPatterns(TokenPattern[] patterns)
    {
        this.patterns.AddRange(patterns);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Lexer Build()
    {
        return new Lexer([.. patterns]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchComment(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.Comment,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchClosingCircumfixDelimiter(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.CloseCircumfixDelimiter,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchOpeningCircumfixDelimiter(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.OpenCircumfixDelimiter,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchInfixDelimiter(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.InfixDelimiter,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchDelimiter(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.Delimiter,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchOperator(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.Operator,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchBooleanFalseLiteral(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.BooleanFalseLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchBooleanTrueLiteral(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.BooleanTrueLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchScientificNotationLiteral(
        int id)
    {
        return Match(
            AuxillaryPatterns.ScientificNotationLiteral(),
            Tokens.ScientificNotationLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchFloatingPointLiteral(
        int id)
    {
        return Match(
            AuxillaryPatterns.FloatingPointLiteral(),
            Tokens.FloatingPointLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchIntegerLiteral(
        int id)
    {
        return Match(
            AuxillaryPatterns.IntegerLiteral(),
            Tokens.IntegerLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchStringLiteral(
        int id)
    {
        return Match(
            AuxillaryPatterns.QuotedStringLiteral(),
            Tokens.StringLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchCharacterLiteral(
        int id)
    {
        return Match(
            AuxillaryPatterns.CharacterLiteral(),
            Tokens.CharacterLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchNullLiteral(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.NullLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchLiteral(
        string pattern,
        int id)
    {
        return Match(pattern, Tokens.Literal, id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchKeyword(
        string pattern,
        int id)
    {
        return Match(pattern, Tokens.Keyword, id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder MatchIdentifier(
        int id)
    {
        return Match(
            AuxillaryPatterns.Identifier(),
            Tokens.Identifier,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder Match(
        string pattern,
        Tokens tokenClass,
        int id)
    {
        patterns.Add(new(
            pattern,
            regexOptions,
            tokenClass,
            id));
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LexerBuilder Match(
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
