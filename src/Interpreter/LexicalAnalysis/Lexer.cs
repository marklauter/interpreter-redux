using System.Runtime.CompilerServices;

namespace Interpreter.LexicalAnalysis;

public sealed class Lexer(
    LinguisticContext context,
    string source)
{
    private int cursor;
    private readonly LinguisticContext context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly string source = source ?? throw new ArgumentNullException(nameof(source));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        return cursor >= source.Length;
    }

    public Token ReadNextToken()
    {
        if (IsEndOfSource())
        {
            return new Token(cursor, 0, TokenType.Eof, String.Empty);
        }

        for (var i = 0; i < context.Length; ++i)
        {
            var language = context[i];
            var match = language.Regex.Match(source, cursor);
            if (match.Success)
            {
                cursor += match.Length;
                return language.Type is TokenType.Whitespace or TokenType.NewLine
                    ? new Token(match.Index, match.Length, language.Type, String.Empty)
                    : new Token(match.Index, match.Length, language.Type, match.Value);
            }
        }

        // todo: parlay this into a real line number and column number
        var line = 0;
        var column = 0;
        return new Token(cursor, 0, TokenType.Error, $"{{\"line\": {line}, \"column\": {column}}}");
    }
}
