using Luthor.Symbols;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Luthor.Tokens;

public sealed partial class Tokenizers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int index, string? reservedWordPattern) AddReservedWordMatcher(
        TerminalSymbolSpec spec,
        PatternMatcher[] matchers,
        Dictionary<TokenType, PatternMatcher> map,
        int index)
    {
        if (!spec.Options.HasFlag(TerminalSymbolOptions.IncludeReservedWords))
        {
            return (index, null);
        }

        if (spec.TryGetReservedWordPattern(out var reservedWordPattern))
        {
            var regex = new Regex($@"\G{reservedWordPattern}", RegexConstants.Options);
            var tokenType = TokenType.ReservedWord;

            Token patternMatcher(string source, int offset)
                => MatchRegex(
                    source,
                    tokenType,
                    offset,
                    regex);

            matchers[index] = patternMatcher;
            map[tokenType] = patternMatcher;

            ++index;
        }

        return (index, reservedWordPattern);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int index, string? booleanLiteralPattern) AddBooleanLiteralMatcher(
        TerminalSymbolSpec spec,
        PatternMatcher[] matchers,
        Dictionary<TokenType, PatternMatcher> map,
        int index)
    {
        if (!spec.Options.HasFlag(TerminalSymbolOptions.IncludeBooleanLiterals))
        {
            return (index, null);
        }

        if (spec.TryGetBooleanLiteralPattern(out var booleanLiteralPattern))
        {
            var regex = new Regex($@"\G(?:{booleanLiteralPattern})", RegexConstants.Options);
            var tokenType = TokenType.BooleanLiteral;

            Token patternMatcher(string source, int offset)
                => MatchRegex(
                    source,
                    tokenType,
                    offset,
                    regex);

            matchers[index] = patternMatcher;
            map[tokenType] = patternMatcher;

            ++index;
        }

        return (index, booleanLiteralPattern);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddIdentifierMatcher(
        TerminalSymbolSpec spec,
        PatternMatcher[] matchers,
        Dictionary<TokenType, PatternMatcher> map,
        int index,
        string? booleanLiteralPattern,
        string? reservedWordPattern)
    {
        if (!spec.Options.HasFlag(TerminalSymbolOptions.IncludeIdentifiers))
        {
            return index;
        }

        var identifierPattern = reservedWordPattern is null
            ? booleanLiteralPattern is null
                ? $@"\G(?:{RegexConstants.Identifiers})"
                : $@"\G(?:{RegexConstants.Identifiers})(?!{booleanLiteralPattern})"
            : booleanLiteralPattern is null
                ? $@"\G(?:{RegexConstants.Identifiers})(?!{reservedWordPattern})"
                : $@"\G(?:{RegexConstants.Identifiers})(?!{reservedWordPattern}|{booleanLiteralPattern})";

        var regex = new Regex(identifierPattern, RegexConstants.Options);
        var tokenType = TokenType.Identifier;

        Token patternMatcher(string source, int offset)
            => MatchRegex(
                source,
                tokenType,
                offset,
                regex);

        matchers[index] = patternMatcher;
        map[tokenType] = patternMatcher;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddNumericLiteralMatcher(
        TerminalSymbolSpec spec,
        PatternMatcher[] matchers,
        Dictionary<TokenType, PatternMatcher> map,
        int index)
    {
        if (!spec.Options.HasFlag(TerminalSymbolOptions.IncludeNumericLiterals))
        {
            return index;
        }

        var regex = NumericLiteralExpression();
        var tokenType = TokenType.NumericLiteral;

        Token patternMatcher(string source, int offset)
            => MatchRegex(
                source,
                tokenType,
                offset,
                regex);

        matchers[index] = patternMatcher;
        map[tokenType] = patternMatcher;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddStringLiteralMatcher(
        TerminalSymbolSpec spec,
        PatternMatcher[] matchers,
        Dictionary<TokenType, PatternMatcher> map,
        int index)
    {
        if (!spec.Options.HasFlag(TerminalSymbolOptions.IncludeStringLiterals))
        {
            return index;
        }

        var regex = StringLiteralExpression();
        var tokenType = TokenType.StringLiteral;

        Token patternMatcher(string source, int offset)
            => MatchRegex(
                source,
                tokenType,
                offset,
                regex);

        matchers[index] = patternMatcher;
        map[tokenType] = patternMatcher;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddCharacterLiteralMatcher(
        TerminalSymbolSpec spec,
        PatternMatcher[] matchers,
        Dictionary<TokenType, PatternMatcher> map,
        int index)
    {
        if (!spec.Options.HasFlag(TerminalSymbolOptions.IncludeCharacterLiterals))
        {
            return index;
        }

        var regex = CharacterLiteralExpression();
        var tokenType = TokenType.CharacterLiteral;

        Token patternMatcher(string source, int offset)
            => MatchRegex(
                source,
                tokenType,
                offset,
                regex);

        matchers[index] = patternMatcher;
        map[tokenType] = patternMatcher;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddNewLineMatcher(
        PatternMatcher[] matchers,
        Dictionary<TokenType, PatternMatcher> map,
        int index)
    {
        var regex = NewLineExpression();
        var tokenType = TokenType.NewLine;

        Token patternMatcher(string source, int offset)
            => MatchRegex(
                source,
                tokenType,
                offset,
                regex);

        matchers[index] = patternMatcher;
        map[tokenType] = patternMatcher;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddWhitespaceMatcher(
        PatternMatcher[] matchers,
        Dictionary<TokenType, PatternMatcher> map,
        int index)
    {
        var regex = WhitespaceExpression();
        var tokenType = TokenType.Whitespace;

        Token patternMatcher(string source, int offset)
            => MatchRegex(
                source,
                tokenType,
                offset,
                regex);

        matchers[index] = patternMatcher;
        map[tokenType] = patternMatcher;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddInfixDelimiterMatcher(
        TerminalSymbolSpec spec,
        PatternMatcher[] matchers,
        Dictionary<TokenType, PatternMatcher> map,
        int index)
    {
        if (!spec.Options.HasFlag(TerminalSymbolOptions.IncludeInfixDelimiters))
        {
            return index;
        }

        if (spec.TryGetInfixDelimiterPattern(out var delimiterPattern))
        {
            var regex = new Regex($@"\G(?:{delimiterPattern})", RegexConstants.Options);
            var tokenType = TokenType.InfixDelimiter;

            Token patternMatcher(string source, int offset)
                => MatchRegex(
                    source,
                    tokenType,
                    offset,
                    regex);

            matchers[index] = patternMatcher;
            map[tokenType] = patternMatcher;

            ++index;
        }

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddCircumfixDelimiterMatcher(
        TerminalSymbolSpec spec,
        PatternMatcher[] matchers,
        Dictionary<TokenType, PatternMatcher> map,
        int index)
    {
        if (!spec.Options.HasFlag(TerminalSymbolOptions.IncludeCircumfixDelimiters))
        {
            return index;
        }

        if (spec.CircumfixDelimiterPairs.Any())
        {
            var openDelimiters = spec
                .CircumfixDelimiterPairs
                .Select(pair => pair.Open)
                .ToArray();

            var closeDelimiters = spec
                .CircumfixDelimiterPairs
                .Select(pair => pair.Close)
                .ToArray();

            Token openMatcher(string source, int offset)
                => MatchCircumfixOpenDelimiter(
                    source,
                    openDelimiters,
                    closeDelimiters,
                    offset);

            matchers[index] = openMatcher;
            map[TokenType.OpenCircumfixDelimiter] = openMatcher;

            ++index;

            Token closeMatcher(string source, int offset)
                => MatchCircumfixCloseDelimiter(
                    source,
                    openDelimiters,
                    closeDelimiters,
                    offset);

            matchers[index] = closeMatcher;
            map[TokenType.CloseCircumfixDelimiter] = closeMatcher;

            ++index;
        }

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddOperatorMatcher(
        TerminalSymbolSpec spec,
        PatternMatcher[] matchers,
        Dictionary<TokenType, PatternMatcher> map,
        int index)
    {
        if (!spec.Options.HasFlag(TerminalSymbolOptions.IncludeOperators))
        {
            return index;
        }

        if (spec.TryGetOperatorPattern(out var operatorPattern))
        {
            var regex = new Regex($@"\G(?:{operatorPattern})", RegexConstants.Options);
            var tokenType = TokenType.Operator;

            Token patternMatcher(string source, int offset)
                => MatchRegex(
                    source,
                    tokenType,
                    offset,
                    regex);

            matchers[index] = patternMatcher;
            map[tokenType] = patternMatcher;

            ++index;
        }

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddCommentMatcher(
        TerminalSymbolSpec spec,
        PatternMatcher[] matchers,
        Dictionary<TokenType, PatternMatcher> map,
        int index)
    {
        if (!spec.Options.HasFlag(TerminalSymbolOptions.IncludeComments))
        {
            return index;
        }

        if (spec.TryGetCommentPattern(out var commentPattern))
        {
            var regex = new Regex($@"\G(?:{commentPattern})", RegexConstants.Options);
            var tokenType = TokenType.Comment;

            Token patternMatcher(string source, int offset)
                => MatchRegex(
                    source,
                    tokenType,
                    offset,
                    regex);

            matchers[index] = patternMatcher;
            map[tokenType] = patternMatcher;

            ++index;
        }

        return index;
    }
}
