using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Lexi;

public class Vocabulary
{
    private readonly List<Pattern> patterns = [];

    private RegexOptions regexOptions =
        RegexOptions.ExplicitCapture |
        RegexOptions.Compiled |
        RegexOptions.Singleline;

    private Vocabulary() { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vocabulary Create()
    {
        return new Vocabulary();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vocabulary Create(RegexOptions regexOptions)
    {
        var builder = new Vocabulary();
        builder.regexOptions |= regexOptions;
        return builder;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary WithPatterns(Pattern[] patterns)
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
    public Vocabulary MatchComment(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.Comment,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchClosingCircumfixDelimiter(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.CloseCircumfixDelimiter,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchOpeningCircumfixDelimiter(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.OpenCircumfixDelimiter,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchInfixDelimiter(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.InfixDelimiter,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchDelimiter(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.Delimiter,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchOperator(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.Operator,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchBooleanFalseLiteral(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.BooleanFalseLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchBooleanTrueLiteral(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.BooleanTrueLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchScientificNotationLiteral(
        int id)
    {
        return Match(
            AuxillaryPatterns.ScientificNotationLiteral(),
            Tokens.ScientificNotationLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchFloatingPointLiteral(
        int id)
    {
        return Match(
            AuxillaryPatterns.FloatingPointLiteral(),
            Tokens.FloatingPointLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchIntegerLiteral(
        int id)
    {
        return Match(
            AuxillaryPatterns.IntegerLiteral(),
            Tokens.IntegerLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchStringLiteral(
        int id)
    {
        return Match(
            AuxillaryPatterns.QuotedStringLiteral(),
            Tokens.StringLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchCharacterLiteral(
        int id)
    {
        return Match(
            AuxillaryPatterns.CharacterLiteral(),
            Tokens.CharacterLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchNullLiteral(
        string pattern,
        int id)
    {
        return Match(
            pattern,
            Tokens.NullLiteral,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchLiteral(
        string pattern,
        int id)
    {
        return Match(pattern, Tokens.Literal, id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchKeyword(
        string pattern,
        int id)
    {
        return Match(pattern, Tokens.Keyword, id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary MatchIdentifier(
        int id)
    {
        return Match(
            AuxillaryPatterns.Identifier(),
            Tokens.Identifier,
            id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vocabulary Match(
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
    public Vocabulary Match(
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
