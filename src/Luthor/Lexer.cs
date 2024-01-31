using Luthor.Context;
using System.Runtime.CompilerServices;

namespace Luthor;

public readonly ref struct Lexer(LexicalContext Context)
{
    private readonly ReadOnlySpan<TokenMatcher> matchers = Context.AsReadOnlySpan();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FirstToken(string source, ref MatchResult match)
    {
        match = new(default, 0, 0, 1);
        ReadToken(source, ref match);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void NextToken(string source, ref MatchResult match)
    {
        ReadToken(source, ref match);
    }

    private void ReadToken(string source, ref MatchResult match)
    {
        var offset = match.NextOffset;
        var lastNewLineOffset = match.LastNewLineOffset;
        var lineNumber = match.LineNumber;

        var length = matchers.Length;
        for (var i = 0; i < length; ++i)
        {
            matchers[i](source, ref match);
            if (match.IsMatch())
            {
                return;
            }
        }

        var column = offset - lastNewLineOffset;
        match = new(
            new Token(offset, 0, Tokens.Error, $"{{\"line\": {lineNumber}, \"column\": {column}}}"),
            offset,
            lastNewLineOffset,
            lineNumber);

        return;
    }
}
