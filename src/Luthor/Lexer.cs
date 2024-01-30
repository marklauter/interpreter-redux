using Luthor.Context;
using Luthor.Tokens;
using System.Runtime.CompilerServices;

namespace Luthor;

/// <summary>
/// Mutable OO style lexer that maintains state internally.
/// </summary>
/// <param name="context">the linguistic context that defines the regex and scan precedence</param>
/// <param name="source">the source to be parsed</param>
public sealed class Lexer(
    LinguisticContext context,
    string source)
{
    private int position;
    private int lastNewLineOffset;
    private int line = 1;
    private readonly LinguisticContext context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly string source = source ?? throw new ArgumentNullException(nameof(source));

    public IEnumerable<Token> ReadTokens()
    {
        var token = ReadToken();
        yield return token;
        while (token.Type != TokenType.EndOfSource)
        {
            token = ReadToken();
            yield return token;
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsEndOfSource()
    {
        return position >= source.Length;
    }

    public Token ReadToken()
    {
        if (IsEndOfSource())
        {
            return new Token(position, 0, TokenType.EndOfSource, String.Empty);
        }

        var length = context.Length;
        var languages = context.AsReadOnlySpan();
        for (var i = 0; i < length; ++i)
        {
            var language = languages[i];
            var match = language.Expression.Match(source, position);
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
