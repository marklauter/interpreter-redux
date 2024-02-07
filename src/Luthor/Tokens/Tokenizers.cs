using Luthor.Exceptions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Luthor.Tokens;

public sealed partial class Tokenizers
{
    private static bool IsEndOfSource(
        int sourceLength,
        int offset,
        out Token? token)
    {
        var isEof = offset >= sourceLength;
        token = isEof
            ? new Token(offset, 0, TokenType.EndOfSource, String.Empty)
            : default;

        return isEof;
    }

    private static Token MatchRegex(
        string source,
        TokenType tokenType,
        int offset,
        Regex regex)
    {
        if (IsEndOfSource(source.Length, offset, out var token))
        {
            return token ?? throw new LexicalErrorException("token was null");
        }

        var match = regex.Match(source, offset);

        var value = tokenType is TokenType.Whitespace or TokenType.NewLine
            ? String.Empty
            : match.Value;

        return match.Success
           ? new(match.Index, match.Length, tokenType, value)
           : new(offset, 0, TokenType.NoMatch | tokenType, String.Empty);
    }

    private static int IndexOfDelimeter(
        ReadOnlySpan<char> source,
        ReadOnlySpan<string> delimiters,
        int offset)
    {
        for (var i = 0; i < delimiters.Length; ++i)
        {
            if (MatchSpan(source, delimiters[i].AsSpan(), offset))
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
            && source[offset..(offset + span.Length)].SequenceEqual(span);
    }

    private static Token MatchCircumfixOpenDelimiter(
        ReadOnlySpan<char> source,
        ReadOnlySpan<string> openDelimiters,
        ReadOnlySpan<string> closeDelimiters,
        int offset)
    {
        var length = source.Length;
        if (IsEndOfSource(length, offset, out var token))
        {
            return token ?? throw new LexicalErrorException("token was null");
        }

        var index = IndexOfDelimeter(
            source,
            openDelimiters,
            offset);

        var tokenType = TokenType.OpenCircumfixDelimiter;

        // no match
        if (index == -1)
        {
            return new(offset, 0, TokenType.NoMatch & tokenType, String.Empty);
        }

        // found open delimiter so read ahead and try to find matching close delimiter
        // no close means lex error
        var openDelimiter = openDelimiters[index].AsSpan();
        var closeDelimiter = closeDelimiters[index].AsSpan();
        var openDelimiterLength = openDelimiter.Length;
        var closeDelimiterLength = closeDelimiter.Length;
        var position = offset + openDelimiterLength;
        var level = 1;

        // while level > 0 and not EOF
        while (level > 0 && position < length)
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

        // level > 0 means no match
        return level == 0
            ? new(offset, openDelimiterLength, tokenType, openDelimiter.ToString())
            : new(offset, 0, TokenType.NoMatch | tokenType, String.Empty);
    }

    private static Token MatchCircumfixCloseDelimiter(
        ReadOnlySpan<char> source,
        ReadOnlySpan<string> openDelimiters,
        ReadOnlySpan<string> closeDelimiters,
        int offset)
    {
        if (IsEndOfSource(source.Length, offset, out var token))
        {
            return token ?? throw new LexicalErrorException("token was null");
        }

        var index = IndexOfDelimeter(
            source,
            closeDelimiters,
            offset);

        var tokenType = TokenType.CloseCircumfixDelimiter;

        // no match
        if (index == -1)
        {
            return new(offset, 0, TokenType.NoMatch & tokenType, String.Empty);
        }

        // found close delimiter so read back and try to find matching open delimiter
        // no open means lex error
        var openDelimiter = openDelimiters[index].AsSpan();
        var closeDelimiter = closeDelimiters[index].AsSpan();
        var openDelimiterLength = openDelimiter.Length;
        var closeDelimiterLength = closeDelimiter.Length;
        var position = offset - 1;
        var level = 1;

        // while level > 0 and not BOF
        while (level > 0 && position >= 0)
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

        // level > 0 means no match
        return level == 0
            ? new(offset, closeDelimiterLength, tokenType, closeDelimiter.ToString())
            : new(offset, 0, TokenType.NoMatch | tokenType, String.Empty);
    }

    [GeneratedRegex(@"\G\b\d+(?:\.?\d+)?(?:[eE]{1}\d+)*(?!\.)\b", RegexConstants.Options)]
    private static partial Regex NumericLiteralExpression();

    [GeneratedRegex(@"\G""(?:[^""\\\n\r]|\\.)*""", RegexConstants.Options)]
    private static partial Regex StringLiteralExpression();

    // todo: need to add char literal pattern for escape codes like \b, \t, \n, \r, \f, \', \", \\, \u0000, \uFFFF
    [GeneratedRegex(@"\G'[^']'", RegexConstants.Options)]
    private static partial Regex CharacterLiteralExpression();

    [GeneratedRegex(@"\G\r\n|[\r\n]", RegexConstants.Options)]
    private static partial Regex NewLineExpression();

    [GeneratedRegex(@"\G\s+", RegexConstants.Options)]
    private static partial Regex WhitespaceExpression();
}
