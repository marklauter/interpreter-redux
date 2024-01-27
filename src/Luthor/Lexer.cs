using Luthor.Context;
using Luthor.Tokens;
using System.Runtime.CompilerServices;

namespace Luthor;

public sealed class Lexer(
    LinguisticContext context,
    string source)
{
    private int position;
    private int lastNewLineOffset;
    private int line;
    private readonly LinguisticContext context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly string source = source ?? throw new ArgumentNullException(nameof(source));

    public IEnumerable<Token> ReadTokens()
    {
        var token = ReadNextToken();
        yield return token;
        while (token.Type != TokenType.Eof)
        {
            token = ReadNextToken();
            yield return token;
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsEndOfSource()
    {
        return position >= source.Length;
    }

    public Token ReadNextToken()
    {
        if (IsEndOfSource())
        {
            return new Token(position, 0, TokenType.Eof, String.Empty);
        }

        var length = context.Length;
        var languages = context.AsSpan();
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
                    ++line;
                }

                return language.Type is TokenType.Whitespace or TokenType.NewLine
                    ? new Token(match.Index, match.Length, language.Type, String.Empty)
                    : new Token(match.Index, match.Length, language.Type, match.Value);
            }
        }

        var column = position - lastNewLineOffset;
        return new Token(position, 0, TokenType.Error, $"{{\"line\": {line}, \"column\": {column}}}");
    }
}
