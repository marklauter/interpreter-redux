using Luthor.Context;
using Luthor.Tokens;
using System.Runtime.CompilerServices;

namespace Luthor;

/// <summary>
/// Immutable functional style lexer that maintains state on the stack.
/// </summary>
/// <param name="context">the linguistic context that defines the regex and scan precedence</param>
// todo: Though the Flexer is slower in unit tests, the expectation is that the FLexer will be faster than the Lexer in real-world use cases because it never touches the heap. Will need to add bench to prove this with a parser.
public readonly ref struct FLexer(LinguisticContext Context)
{
    private readonly ReadOnlySpan<LinguisticExpression> languages = Context.AsReadOnlySpan();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FLexResult ReadToken(string source)
    {
        return ReadToken(source, 0, 0, 1);
    }

    public FLexResult ReadToken(string source, int position, int lastNewLineOffset, int lineNumber)
    {
        // is end of source?
        if (position >= source.Length)
        {
            return new(
                new RefToken(position, 0, TokenType.Eof, String.Empty),
                position,
                lastNewLineOffset,
                lineNumber);
        }

        var length = languages.Length;
        for (var i = 0; i < length; ++i)
        {
            var language = languages[i];
            var match = language.Regex.Match(source, position);
            if (match.Success)
            {
                position += match.Length;
                if (language.Type == TokenType.NewLine)
                {
                    lastNewLineOffset = match.Index; // for error tracking. it helps establish the column where the error occurred
                    ++lineNumber;
                }

                return language.Type is TokenType.Whitespace or TokenType.NewLine
                    ? new(
                        new RefToken(match.Index, match.Length, language.Type, String.Empty),
                        position,
                        lastNewLineOffset,
                        lineNumber)
                    : new(
                        new RefToken(match.Index, match.Length, language.Type, match.Value),
                        position,
                        lastNewLineOffset,
                        lineNumber);
            }
        }

        var column = position - lastNewLineOffset;
        return new(
            new RefToken(position, 0, TokenType.Error, $"{{\"line\": {lineNumber}, \"column\": {column}}}"),
            position,
            lastNewLineOffset,
            lineNumber);
    }
}
