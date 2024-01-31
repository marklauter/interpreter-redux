using Luthor.Context;
using System.Runtime.CompilerServices;

namespace Luthor;

/// <summary>
/// Immutable functional style lexer that maintains state on the stack.
/// </summary>
/// <param name="context">the linguistic context that defines the regex and scan precedence</param>
// todo: Though the Flexer is slower in unit tests, the expectation is that the FLexer will be faster than the Lexer in real-world use cases because it never touches the heap. Will need to add bench to prove this with a parser.
public readonly ref struct Lexer(LexicalContext Context)
{
    private readonly ReadOnlySpan<TokenReader> readers = Context.AsReadOnlySpan();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadTokenResult ReadToken(string source)
    {
        return ReadToken(source, 0, 0, 1);
    }

    public ReadTokenResult ReadToken(string source, int offset, int lastNewLineOffset, int lineNumber)
    {
        var length = readers.Length;
        for (var i = 0; i < length; ++i)
        {
            var result = readers[i]
                .Invoke(source, offset, lastNewLineOffset, lineNumber);

            if (result.IsMatch)
            {
                return result;
            }
        }

        var column = offset - lastNewLineOffset;
        return new(
            new Token(offset, 0, Tokens.Error, $"{{\"line\": {lineNumber}, \"column\": {column}}}"),
            offset,
            lastNewLineOffset,
            lineNumber);
    }
}
