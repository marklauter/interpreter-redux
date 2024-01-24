using Interpreter.LexicalAnalysis.Exceptions;
using System.Runtime.CompilerServices;

namespace Interpreter.LexicalAnalysis;

public sealed class Lexer(
    LanguageCollection languages,
    string source)
{
    private int cursor;
    private readonly LanguageCollection languages = languages ?? throw new ArgumentNullException(nameof(languages));
    private readonly string source = source ?? throw new ArgumentNullException(nameof(source));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<Token> ReadTokens()
    {
        while (!Eof())
        {
            yield return ReadNextToken();
        }
    }

    public bool Eof()
    {
        return cursor >= source.Length;
    }

    public Token ReadNextToken()
    {
        if (Eof())
        {
            return new Token(cursor, 0, TokenType.Eof, String.Empty);
        }

        for (var i = 0; i < languages.Length; ++i)
        {
            var language = languages[i];
            var match = language.Regex.Match(source, cursor);
            if (match.Success)
            {
                cursor += match.Length;
                return language.Type is TokenType.Whitespace or TokenType.NewLine
                    ? new Token(match.Index, match.Length, language.Type, String.Empty)
                    : new Token(match.Index, match.Length, language.Type, match.Value);
            }
        }

        // todo: parlay this into a line number and column number
        throw new SyntaxErrorException($"Unexpected token at {cursor}: '{source[cursor..]}'");
    }
}
