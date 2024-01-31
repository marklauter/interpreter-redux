using Luthor.Context;
using System.Runtime.CompilerServices;

namespace Luthor;

public readonly ref struct Lexer(LexicalContext Context)
{
    private readonly ReadOnlySpan<TokenReader> readers = Context.AsReadOnlySpan();

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadTokenResult ReadToken(string source)
    {
        return ReadToken(source, 0, 0, 1);
    }
}
