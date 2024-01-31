using Luthor.Exceptions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Luthor.Context;

public partial class LexicalContext
{
    private const RegexOptions RegularExpressionOptions =
        RegexOptions.CultureInvariant |
        RegexOptions.ExplicitCapture |
        RegexOptions.Compiled |
        RegexOptions.Singleline;

    private static ReadTokenResult MatchRegex(
        string source,
        int offset,
        int lastNewLineOffset,
        int lineNumber,
        Tokens tokenType,
        Regex regex)
    {
        if (offset >= source.Length)
        {
            return new(
                new Token(offset, 0, Tokens.EndOfSource, String.Empty),
                offset,
                lastNewLineOffset,
                lineNumber);
        }

        var match = regex.Match(source, offset);

        return match.Success
            ? new ReadTokenResult(
                new(match.Index, match.Length, tokenType, match.Value),
                match.Index + match.Length,
                lastNewLineOffset,
                lineNumber)
            : new ReadTokenResult(
                new(offset, 0, Tokens.NoMatch | tokenType, String.Empty),
                offset,
                lastNewLineOffset,
                lineNumber);
    }

    private static ReadTokenResult MatchNewLine(
        string source,
        int offset,
        int lastNewLineOffset,
        int lineNumber,
        Regex regex)
    {
        if (offset >= source.Length)
        {
            return new(
                new(offset, 0, Tokens.EndOfSource, String.Empty),
                offset,
                lastNewLineOffset,
                lineNumber);
        }

        var tokenType = Tokens.NewLine;
        var match = regex.Match(source, offset);

        if (!match.Success)
        {
            return new(
                new(offset, 0, Tokens.NoMatch & tokenType, String.Empty),
                offset,
                lastNewLineOffset,
                lineNumber);
        }

        // for error tracking. it helps establish the column where the error occurred
        lastNewLineOffset = match.Index;
        ++lineNumber;

        return new(
            new(match.Index, match.Length, tokenType, String.Empty),
            match.Index + match.Length,
            lastNewLineOffset,
            lineNumber);
    }

    private static ReadTokenResult MatchWhitespace(
        string source,
        int offset,
        int lastNewLineOffset,
        int lineNumber,
        Regex regex)
    {
        if (offset >= source.Length)
        {
            return new(
                new(offset, 0, Tokens.EndOfSource, String.Empty),
                offset,
                lastNewLineOffset,
                lineNumber);
        }

        var tokenType = Tokens.Whitespace;
        var match = regex.Match(source, offset);

        return match.Success
            ? new ReadTokenResult(
                new(match.Index, match.Length, tokenType, String.Empty),
                match.Index + match.Length,
                lastNewLineOffset,
                lineNumber)
            : new ReadTokenResult(
                new(offset, 0, Tokens.NoMatch & tokenType, String.Empty),
                offset,
                lastNewLineOffset,
                lineNumber);
    }

    private static int IndexOfDelimeter(
        ReadOnlySpan<char> source,
        int offset,
        ReadOnlySpan<string> delimiters)
    {
        for (var i = 0; i < delimiters.Length; ++i)
        {
            if (MatchSpan(source, delimiters[i], offset))
            {
                return i;
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool MatchSpan(
        ReadOnlySpan<char> source,
        ReadOnlySpan<char> span,
        int offset)
    {
        return offset + span.Length <= source.Length
            && source[offset..].SequenceEqual(span);
    }

    private static ReadTokenResult MatchCircumfixOpenDelimiter(
        ReadOnlySpan<char> source,
        int offset,
        int lastNewLineOffset,
        int lineNumber,
        ReadOnlySpan<string> openDelimiters,
        ReadOnlySpan<string> closeDelimiters)
    {
        if (offset >= source.Length)
        {
            return new(
                new(offset, 0, Tokens.EndOfSource, String.Empty),
                offset,
                lastNewLineOffset,
                lineNumber);
        }

        var index = IndexOfDelimeter(
            source,
            offset,
            openDelimiters);

        var tokenType = Tokens.OpenCircumfixDelimiter;

        // no match, return success = false
        if (index == -1)
        {
            return new(
                new(offset, 0, Tokens.NoMatch & tokenType, String.Empty),
                offset,
                lastNewLineOffset,
                lineNumber);
        }

        // found open delimiter so read ahead and try to find matching close delimiter
        // no close means lex error
        var openDelimiter = openDelimiters[index];
        var closeDelimiter = closeDelimiters[index];
        var openDelimiterLength = openDelimiter.Length;
        var closeDelimiterLength = closeDelimiter.Length;
        var position = offset + openDelimiterLength;
        var level = 1;

        // while level > 0 and not EOF
        while (level > 0 && position < source.Length)
        {
            // when a close is encountered, decrement level
            if (MatchSpan(source, closeDelimiter, position))
            {
                --level;
                position += closeDelimiterLength;
                continue;
            }

            // when an open is encountered, increment level
            if (MatchSpan(source, openDelimiter, position))
            {
                ++level;
                position += openDelimiterLength;
                continue;
            }

            ++position;
        }

        // level > 0 means no match, return success = false
        return level != 0
            ? throw new CircumfixDelimiterMismatchException($"no matching close token for {openDelimiter}. Expected {closeDelimiter}")
            : new(
                new(offset, openDelimiterLength, tokenType, openDelimiter),
                offset + openDelimiterLength,
                lastNewLineOffset,
                lineNumber);
    }

    private static ReadTokenResult MatchCircumfixCloseDelimiter(
        ReadOnlySpan<char> source,
        int offset,
        int lastNewLineOffset,
        int lineNumber,
        ReadOnlySpan<string> openDelimiters,
        ReadOnlySpan<string> closeDelimiters)
    {
        if (offset >= source.Length)
        {
            return new(
                new(offset, 0, Tokens.EndOfSource, String.Empty),
                offset,
                lastNewLineOffset,
                lineNumber);
        }

        var index = IndexOfDelimeter(
            source,
            offset,
            closeDelimiters);

        var tokenType = Tokens.CloseCircumfixDelimiter;

        // no match, return success = false
        if (index == -1)
        {
            return new(
                new(offset, 0, Tokens.NoMatch & tokenType, String.Empty),
                offset,
                lastNewLineOffset,
                lineNumber);
        }

        // found close delimiter so read back and try to find matching open delimiter
        // no open means lex error
        var openDelimiter = openDelimiters[index];
        var closeDelimiter = closeDelimiters[index];
        var openDelimiterLength = openDelimiter.Length;
        var closeDelimiterLength = closeDelimiter.Length;
        var position = offset;
        var level = 1;

        // while level > 0 and not BOF
        while (level > 0 && position > 0)
        {
            // when an open is encountered, decrement level
            if (MatchSpan(source, openDelimiter, position))
            {
                --level;
                position -= openDelimiterLength;
                continue;
            }

            // when a close is encountered, increment level
            if (MatchSpan(source, closeDelimiter, position))
            {
                ++level;
                position -= closeDelimiterLength;
                continue;
            }

            --position;
        }

        // level > 0 means no match, return success = false
        return level > 0
            ? throw new CircumfixDelimiterMismatchException($"no matching open token for {closeDelimiter}. Expected {openDelimiter}")
            : new(
                new(offset, openDelimiterLength, tokenType, openDelimiter),
                offset + openDelimiterLength,
                lastNewLineOffset,
                lineNumber);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int index, string? reservedWordPattern) AddReservedWordReader(
        LanguageSpecification spec,
        TokenReader[] readers,
        Dictionary<Tokens, TokenReader> map,
        int index)
    {
        if (spec.TryGetReservedWordPattern(out var reservedWordPattern))
        {
            var regex = new Regex($@"\G(?:{reservedWordPattern})", RegularExpressionOptions);

            ReadTokenResult reader(string source, int offset, int lastNewLineOffset, int lineNumber)
                => MatchRegex(
                    source,
                    offset,
                    lastNewLineOffset,
                    lineNumber,
                    Tokens.ReservedWord,
                    regex);

            readers[index] = reader;
            map[Tokens.ReservedWord] = reader;

            ++index;
        }

        return (index, reservedWordPattern);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int index, string? booleanLiteralPattern) AddBooleanLiterals(
        LanguageSpecification spec,
        TokenReader[] readers,
        Dictionary<Tokens, TokenReader> map,
        int index)
    {
        if (spec.TryGetBooleanLiteralPattern(out var booleanLiteralPattern))
        {
            var regex = new Regex($@"\G(?:{booleanLiteralPattern})", RegularExpressionOptions);
            ReadTokenResult reader(string source, int offset, int lastNewLineOffset, int lineNumber)
                => MatchRegex(
                    source,
                    offset,
                    lastNewLineOffset,
                    lineNumber,
                    Tokens.BooleanLiteral,
                    regex);

            readers[index] = reader;
            map[Tokens.BooleanLiteral] = reader;


            ++index;
        }

        return (index, booleanLiteralPattern);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddIdentifiers(
        TokenReader[] readers,
        Dictionary<Tokens, TokenReader> map,
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

        ReadTokenResult reader(string source, int offset, int lastNewLineOffset, int lineNumber)
            => MatchRegex(
                source,
                offset,
                lastNewLineOffset,
                lineNumber,
                Tokens.Identifier,
                regex);

        readers[index] = reader;
        map[Tokens.Identifier] = reader;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddNumericLiterals(
        TokenReader[] readers,
        Dictionary<Tokens, TokenReader> map,
        int index)
    {
        var regex = NumericLiteralExpression();
        ReadTokenResult reader(string source, int offset, int lastNewLineOffset, int lineNumber)
            => MatchRegex(
                source,
                offset,
                lastNewLineOffset,
                lineNumber,
                Tokens.NumericLiteral,
                regex);

        readers[index] = reader;
        map[Tokens.NumericLiteral] = reader;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddStringLiterals(
        TokenReader[] readers,
        Dictionary<Tokens, TokenReader> map,
        int index)
    {
        var regex = StringLiteralExpression();
        ReadTokenResult reader(string source, int offset, int lastNewLineOffset, int lineNumber)
            => MatchRegex(
                source,
                offset,
                lastNewLineOffset,
                lineNumber,
                Tokens.StringLiteral,
                regex);

        readers[index] = reader;
        map[Tokens.StringLiteral] = reader;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddCharacterLiterals(
        TokenReader[] readers,
        Dictionary<Tokens, TokenReader> map,
        int index)
    {
        var regex = CharacterLiteralExpression();
        ReadTokenResult reader(string source, int offset, int lastNewLineOffset, int lineNumber)
            => MatchRegex(
                source,
                offset,
                lastNewLineOffset,
                lineNumber,
                Tokens.CharacterLiteral,
                regex);

        readers[index] = reader;
        map[Tokens.CharacterLiteral] = reader;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddNewLine(
        TokenReader[] readers,
        Dictionary<Tokens, TokenReader> map,
        int index)
    {
        var regex = NewLineExpression();
        ReadTokenResult reader(string source, int offset, int lastNewLineOffset, int lineNumber)
            => MatchNewLine(
                source,
                offset,
                lastNewLineOffset,
                lineNumber,
                regex);

        readers[index] = reader;
        map[Tokens.NewLine] = reader;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddWhitespace(
        TokenReader[] readers,
        Dictionary<Tokens, TokenReader> map,
        int index)
    {
        var regex = WhitespaceExpression();
        ReadTokenResult reader(string source, int offset, int lastNewLineOffset, int lineNumber)
            => MatchWhitespace(
                source,
                offset,
                lastNewLineOffset,
                lineNumber,
                regex);

        readers[index] = reader;
        map[Tokens.Whitespace] = reader;

        return index + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddInfixDelimiters(
        LanguageSpecification spec,
        TokenReader[] readers,
        Dictionary<Tokens, TokenReader> map,
        int index)
    {
        if (spec.TryGetInfixDelimiterPattern(out var delimiterPattern))
        {
            var regex = new Regex($@"\G(?:{delimiterPattern})", RegularExpressionOptions);

            ReadTokenResult reader(string source, int offset, int lastNewLineOffset, int lineNumber)
                => MatchRegex(
                    source,
                    offset,
                    lastNewLineOffset,
                    lineNumber,
                    Tokens.InfixDelimiter,
                    regex);

            readers[index] = reader;
            map[Tokens.InfixDelimiter] = reader;

            ++index;
        }

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddCircumfixDelimiters(
        LanguageSpecification spec,
        TokenReader[] readers,
        Dictionary<Tokens, TokenReader> map,
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

            ReadTokenResult openReader(string source, int offset, int lastNewLineOffset, int lineNumber)
                => MatchCircumfixOpenDelimiter(
                    source,
                    offset,
                    lastNewLineOffset,
                    lineNumber,
                    openDelimiters,
                    closeDelimiters);

            readers[index] = openReader;
            map[Tokens.OpenCircumfixDelimiter] = openReader;

            ++index;

            ReadTokenResult closeReader(string source, int offset, int lastNewLineOffset, int lineNumber)
                => MatchCircumfixCloseDelimiter(
                    source,
                    offset,
                    lastNewLineOffset,
                    lineNumber,
                    openDelimiters,
                    closeDelimiters);

            readers[index] = closeReader;
            map[Tokens.CloseCircumfixDelimiter] = closeReader;

            ++index;
        }

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddOperators(
        LanguageSpecification spec,
        TokenReader[] readers,
        Dictionary<Tokens, TokenReader> map,
        int index)
    {
        if (spec.TryGetOperatorPattern(out var operatorPattern))
        {
            var regex = new Regex($@"\G(?:{operatorPattern})", RegularExpressionOptions);

            ReadTokenResult reader(string source, int offset, int lastNewLineOffset, int lineNumber)
                => MatchRegex(
                    source,
                    offset,
                    lastNewLineOffset,
                    lineNumber,
                    Tokens.Operator,
                    regex);

            readers[index] = reader;
            map[Tokens.Operator] = reader;

            ++index;
        }

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AddComments(
        LanguageSpecification spec,
        TokenReader[] readers,
        Dictionary<Tokens, TokenReader> map,
        int index)
    {
        if (spec.TryGetCommentPattern(out var commentPattern))
        {
            var regex = new Regex($@"\G(?:{commentPattern})", RegularExpressionOptions);

            ReadTokenResult reader(string source, int offset, int lastNewLineOffset, int lineNumber)
                => MatchRegex(
                    source,
                    offset,
                    lastNewLineOffset,
                    lineNumber,
                    Tokens.Comment,
                    regex);

            readers[index] = reader;
            map[Tokens.Comment] = reader;

            ++index;
        }

        return index;
    }

    [GeneratedRegex(@"\G(?:\b\d+(?:\.{1}\d+)?\b)", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
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
