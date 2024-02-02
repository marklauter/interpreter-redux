using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Luthor.Context;

public sealed partial class LexicalContext
{
    private const RegexOptions RegularExpressionOptions =
        RegexOptions.CultureInvariant |
        RegexOptions.ExplicitCapture |
        RegexOptions.Compiled |
        RegexOptions.Singleline;

    private static bool CheckEndOfSource(ReadOnlySpan<char> source, ref MatchResult match)
    {
        var isEof = match.NextOffset >= source.Length;
        if (isEof)
        {
            match = new(
                new Token(match.NextOffset, 0, Tokens.EndOfSource, String.Empty),
                match.NextOffset,
                match.LastNewLineOffset,
                match.LineNumber);
        }

        return isEof;
    }

    private static void MatchRegex(
        string source,
        Tokens tokenType,
        Regex regex,
        ref MatchResult match)
    {
        if (CheckEndOfSource(source, ref match))
        {
            return;
        }

        var regexMatch = regex.Match(source, match.NextOffset);

        match = regexMatch.Success
            ? new(
                new(regexMatch.Index, regexMatch.Length, tokenType, regexMatch.Value),
                regexMatch.Index + regexMatch.Length,
                match.LastNewLineOffset,
                match.LineNumber)
            : new(
                new(match.NextOffset, 0, Tokens.NoMatch | tokenType, String.Empty),
                match.NextOffset,
                match.LastNewLineOffset,
                match.LineNumber);
    }

    private static void MatchNewLine(
        string source,
        Regex regex,
        ref MatchResult match)
    {
        if (CheckEndOfSource(source, ref match))
        {
            return;
        }

        var tokenType = Tokens.NewLine;
        var regexMatch = regex.Match(source, match.NextOffset);

        if (!regexMatch.Success)
        {
            match = new(
                new(match.NextOffset, 0, Tokens.NoMatch & tokenType, String.Empty),
                match.NextOffset,
                match.LastNewLineOffset,
                match.LineNumber);

            return;
        }

        match = new(
            new(regexMatch.Index, regexMatch.Length, tokenType, String.Empty),
            regexMatch.Index + regexMatch.Length,
            // for error tracking. it helps establish the column where the error occurred
            regexMatch.Index,
            match.LineNumber + 1);
    }

    private static void MatchWhitespace(
        string source,
        Regex regex,
        ref MatchResult match)
    {
        if (CheckEndOfSource(source, ref match))
        {
            return;
        }

        var tokenType = Tokens.Whitespace;
        var regexMatch = regex.Match(source, match.NextOffset);

        match = regexMatch.Success
            ? new(
                new(regexMatch.Index, regexMatch.Length, tokenType, String.Empty),
                regexMatch.Index + regexMatch.Length,
                match.LastNewLineOffset,
                match.LineNumber)
            : new(
                new(match.NextOffset, 0, Tokens.NoMatch & tokenType, String.Empty),
                match.NextOffset,
                match.LastNewLineOffset,
                match.LineNumber);
    }

    private static int IndexOfDelimeter(
        ref ReadOnlySpan<char> source,
        int offset,
        ref ReadOnlySpan<string> delimiters)
    {
        for (var i = 0; i < delimiters.Length; ++i)
        {
            var delimiter = delimiters[i].AsSpan();
            if (MatchSpan(ref source, ref delimiter, offset))
            {
                return i;
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool MatchSpan(
        ref ReadOnlySpan<char> source,
        ref ReadOnlySpan<char> span,
        int offset)
    {
        return offset + span.Length <= source.Length
            && source[offset..(offset + span.Length)].SequenceEqual(span);
    }

    private static void MatchCircumfixOpenDelimiter(
        ReadOnlySpan<char> source,
        ReadOnlySpan<string> openDelimiters,
        ReadOnlySpan<string> closeDelimiters,
        ref MatchResult match)
    {
        if (CheckEndOfSource(source, ref match))
        {
            return;
        }

        var index = IndexOfDelimeter(
            ref source,
            match.NextOffset,
            ref openDelimiters);

        var tokenType = Tokens.OpenCircumfixDelimiter;

        // no match
        if (index == -1)
        {
            match = new(
                new(match.NextOffset, 0, Tokens.NoMatch & tokenType, String.Empty),
                match.NextOffset,
                match.LastNewLineOffset,
                match.LineNumber);

            return;
        }

        // found open delimiter so read ahead and try to find matching close delimiter
        // no close means lex error
        var openDelimiter = openDelimiters[index].AsSpan();
        var closeDelimiter = closeDelimiters[index].AsSpan();
        var openDelimiterLength = openDelimiter.Length;
        var closeDelimiterLength = closeDelimiter.Length;
        var position = match.NextOffset + openDelimiterLength;
        var level = 1;

        // while level > 0 and not EOF
        while (level > 0 && position < source.Length)
        {
            // when a close is encountered, decrement level
            if (MatchSpan(ref source, ref closeDelimiter, position))
            {
                --level;
                position += closeDelimiterLength;
                continue;
            }

            // when an open is encountered, increment level
            if (MatchSpan(ref source, ref openDelimiter, position))
            {
                ++level;
                position += openDelimiterLength;
                continue;
            }

            ++position;
        }

        // level > 0 means no match, return success = false
        match = level == 0
            ? new(
                new(match.NextOffset, openDelimiterLength, tokenType, openDelimiter.ToString()),
                match.NextOffset + openDelimiterLength,
                match.LastNewLineOffset,
                match.LineNumber)
            : new(
                new(match.NextOffset, 0, Tokens.NoMatch | tokenType, String.Empty),
                match.NextOffset,
                match.LastNewLineOffset,
                match.LineNumber);
    }

    private static void MatchCircumfixCloseDelimiter(
        ReadOnlySpan<char> source,
        ReadOnlySpan<string> openDelimiters,
        ReadOnlySpan<string> closeDelimiters,
        ref MatchResult match)
    {
        if (CheckEndOfSource(source, ref match))
        {
            return;
        }

        var index = IndexOfDelimeter(
            ref source,
            match.NextOffset,
            ref closeDelimiters);

        var tokenType = Tokens.CloseCircumfixDelimiter;

        // no match
        if (index == -1)
        {
            match = new(
                new(match.NextOffset, 0, Tokens.NoMatch & tokenType, String.Empty),
                match.NextOffset,
                match.LastNewLineOffset,
                match.LineNumber);

            return;
        }

        // found close delimiter so read back and try to find matching open delimiter
        // no open means lex error
        var openDelimiter = openDelimiters[index].AsSpan();
        var closeDelimiter = closeDelimiters[index].AsSpan();
        var openDelimiterLength = openDelimiter.Length;
        var closeDelimiterLength = closeDelimiter.Length;
        var position = match.NextOffset - 1;
        var level = 1;

        // while level > 0 and not BOF
        while (level > 0 && position > 0)
        {
            // when an open is encountered, decrement level
            if (MatchSpan(ref source, ref openDelimiter, position))
            {
                --level;
                position -= openDelimiterLength;
                continue;
            }

            // when a close is encountered, increment level
            if (MatchSpan(ref source, ref closeDelimiter, position))
            {
                ++level;
                position -= closeDelimiterLength;
                continue;
            }

            --position;
        }

        // level > 0 means no match
        match = level == 0
            ? new(
                new(match.NextOffset, closeDelimiterLength, tokenType, closeDelimiter.ToString()),
                match.NextOffset + closeDelimiterLength,
                match.LastNewLineOffset,
                match.LineNumber)
            : new(
                new(match.NextOffset, 0, Tokens.NoMatch | tokenType, String.Empty),
                match.NextOffset,
                match.LastNewLineOffset,
                match.LineNumber);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int index, string? reservedWordPattern) AddReservedWordMatcher(
        LanguageSpecification spec,
        TokenMatcher[] matchers,
        Dictionary<Tokens, TokenMatcher> map,
        int index)
    {
        if (spec.TryGetReservedWordPattern(out var reservedWordPattern))
        {
            var regex = new Regex($@"\G(?:{reservedWordPattern})", RegularExpressionOptions);

            void matcher(string source, ref MatchResult match)
                => MatchRegex(
                    source,
                    Tokens.ReservedWord,
                    regex,
                    ref match);

            matchers[index] = matcher;
            map[Tokens.ReservedWord] = matcher;

            ++index;
        }

        return (index, reservedWordPattern);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int index, string? booleanLiteralPattern) AddBooleanLiteralMatcher(
        LanguageSpecification spec,
        TokenMatcher[] matchers,
        Dictionary<Tokens, TokenMatcher> map,
        int index)
    {
        if (spec.TryGetBooleanLiteralPattern(out var booleanLiteralPattern))
        {
            var regex = new Regex($@"\G(?:{booleanLiteralPattern})", RegularExpressionOptions);

            void matcher(string source, ref MatchResult match)
                => MatchRegex(
                    source,
                    Tokens.BooleanLiteral,
                    regex,
                    ref match);

            matchers[index] = matcher;
            map[Tokens.BooleanLiteral] = matcher;

            ++index;
        }

        return (index, booleanLiteralPattern);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddIdentifierMatcher(
        TokenMatcher[] matchers,
        Dictionary<Tokens, TokenMatcher> map,
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

        var regex = new Regex(identifierPattern, RegularExpressionOptions);

        void matcher(string source, ref MatchResult match)
            => MatchRegex(
                source,
                Tokens.Identifier,
                regex,
                ref match);

        matchers[index] = matcher;
        map[Tokens.Identifier] = matcher;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddNumericLiteralMatcher(
        TokenMatcher[] matchers,
        Dictionary<Tokens, TokenMatcher> map,
        int index)
    {
        var regex = NumericLiteralExpression();

        void matcher(string source, ref MatchResult match)
            => MatchRegex(
                source,
                Tokens.NumericLiteral,
                regex,
                ref match);

        matchers[index] = matcher;
        map[Tokens.NumericLiteral] = matcher;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddStringLiteralMatcher(
        TokenMatcher[] matchers,
        Dictionary<Tokens, TokenMatcher> map,
        int index)
    {
        var regex = StringLiteralExpression();

        void matcher(string source, ref MatchResult match)
            => MatchRegex(
                source,
                Tokens.StringLiteral,
                regex,
                ref match);

        matchers[index] = matcher;
        map[Tokens.StringLiteral] = matcher;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddCharacterLiteralMatcher(
        TokenMatcher[] matchers,
        Dictionary<Tokens, TokenMatcher> map,
        int index)
    {
        var regex = CharacterLiteralExpression();

        void matcher(string source, ref MatchResult match)
            => MatchRegex(
                source,
                Tokens.CharacterLiteral,
                regex,
                ref match);

        matchers[index] = matcher;
        map[Tokens.CharacterLiteral] = matcher;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddNewLineMatcher(
        TokenMatcher[] matchers,
        Dictionary<Tokens, TokenMatcher> map,
        int index)
    {
        var regex = NewLineExpression();
        void matcher(string source, ref MatchResult match)
            => MatchNewLine(
                source,
                regex,
                ref match);

        matchers[index] = matcher;
        map[Tokens.NewLine] = matcher;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddWhitespaceMatcher(
        TokenMatcher[] matchers,
        Dictionary<Tokens, TokenMatcher> map,
        int index)
    {
        var regex = WhitespaceExpression();
        void matcher(string source, ref MatchResult match)
            => MatchWhitespace(
                source,
                regex,
                ref match);

        matchers[index] = matcher;
        map[Tokens.Whitespace] = matcher;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddInfixDelimiterMatcher(
        LanguageSpecification spec,
        TokenMatcher[] matchers,
        Dictionary<Tokens, TokenMatcher> map,
        int index)
    {
        if (spec.TryGetInfixDelimiterPattern(out var delimiterPattern))
        {
            var regex = new Regex($@"\G(?:{delimiterPattern})", RegularExpressionOptions);

            void matcher(string source, ref MatchResult match)
                => MatchRegex(
                    source,
                    Tokens.InfixDelimiter,
                    regex,
                    ref match);

            matchers[index] = matcher;
            map[Tokens.InfixDelimiter] = matcher;

            ++index;
        }

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddCircumfixDelimiterMatcher(
        LanguageSpecification spec,
        TokenMatcher[] matchers,
        Dictionary<Tokens, TokenMatcher> map,
        int index)
    {
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

            void openMatcher(string source, ref MatchResult match)
                => MatchCircumfixOpenDelimiter(
                    source,
                    openDelimiters,
                    closeDelimiters,
                    ref match);

            matchers[index] = openMatcher;
            map[Tokens.OpenCircumfixDelimiter] = openMatcher;

            ++index;

            void closeMatcher(string source, ref MatchResult match)
                => MatchCircumfixCloseDelimiter(
                    source,
                    openDelimiters,
                    closeDelimiters,
                    ref match);

            matchers[index] = closeMatcher;
            map[Tokens.CloseCircumfixDelimiter] = closeMatcher;

            ++index;
        }

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddOperatorMatcher(
        LanguageSpecification spec,
        TokenMatcher[] matchers,
        Dictionary<Tokens, TokenMatcher> map,
        int index)
    {
        if (spec.TryGetOperatorPattern(out var operatorPattern))
        {
            var regex = new Regex($@"\G(?:{operatorPattern})", RegularExpressionOptions);

            void matcher(string source, ref MatchResult match)
                => MatchRegex(
                    source,
                    Tokens.Operator,
                    regex,
                    ref match);

            matchers[index] = matcher;
            map[Tokens.Operator] = matcher;

            ++index;
        }

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddCommentMatcher(
        LanguageSpecification spec,
        TokenMatcher[] matchers,
        Dictionary<Tokens, TokenMatcher> map,
        int index)
    {
        if (spec.TryGetCommentPattern(out var commentPattern))
        {
            var regex = new Regex($@"\G(?:{commentPattern})", RegularExpressionOptions);

            void matcher(string source, ref MatchResult match)
                => MatchRegex(
                    source,
                    Tokens.Comment,
                    regex,
                    ref match);

            matchers[index] = matcher;
            map[Tokens.Comment] = matcher;

            ++index;
        }

        return index;
    }

    [GeneratedRegex(@"\G\b\d+(?:\.?\d+)?(?:[eE]{1}\d+)*(?!\.)\b", RegularExpressionOptions)]
    private static partial Regex NumericLiteralExpression();

    [GeneratedRegex(@"\G""(?:[^""\\\n\r]|\\.)*""", RegularExpressionOptions)]
    private static partial Regex StringLiteralExpression();

    // todo: need to add char literal pattern for escape codes like \b, \t, \n, \r, \f, \', \", \\, \u0000, \uFFFF
    [GeneratedRegex(@"\G'[^']'", RegularExpressionOptions)]
    private static partial Regex CharacterLiteralExpression();

    [GeneratedRegex(@"\G\r\n|[\r\n]", RegularExpressionOptions)]
    private static partial Regex NewLineExpression();

    [GeneratedRegex(@"\G\s+", RegularExpressionOptions)]
    private static partial Regex WhitespaceExpression();
}
