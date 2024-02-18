using Luthor.Tokens;
using System.Runtime.CompilerServices;

namespace Luthor;

public sealed class Lexer(
    Tokenizers tokenizers,
    string source)
{
    private readonly Tokenizers tokenizers = tokenizers ?? throw new ArgumentNullException(nameof(tokenizers));
    private readonly string source = source ?? throw new ArgumentNullException(nameof(source));
    private readonly List<int> newLineOffsets = [];
    private int offset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (int line, int column) LineAndColumn(int offset)
    {
        var offsets = newLineOffsets.ToArray().AsSpan();
        for (var line = 0; line < offsets.Length; ++line)
        {
            var newLineOffset = offsets[line];
            if (offset > newLineOffset)
            {
                return (line, offset - newLineOffset);
            }
        }

        return (1, offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadSymbol(Token token)
    {
        if (token.IsEndOfSource())
        {
            return "EOF";
        }

        if (token.IsError())
        {
            var (lineNumber, column) = LineAndColumn(token.Offset);
            return $"{{\"line\": {lineNumber}, \"column\": {column}}}";
        }

        return source[token.Offset..(token.Offset + token.Length)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadSymbol(string source, Token token)
    {
        return token.IsEndOfSource()
            ? "EOF"
            : source[token.Offset..(token.Offset + token.Length)];
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "span created with collection initialization can't be exposed outside the function")]
    public Token[] Tokens()
    {
        var tokens = new List<Token>();
        var token = NextToken();
        while (!token.IsEndOfSource()
            && !token.IsError())
        {
            tokens.Add(token);
            token = NextToken();
        }

        tokens.Add(token);

        offset = 0;

        return tokens
            .ToArray();
    }

    public Token NextToken()
    {
        if (offset >= source.Length)
        {
            return new Token(offset, 0, TokenType.EndOfSource);
        }

        var length = tokenizers.Length;
        for (var i = 0; i < length; ++i)
        {
            var match = tokenizers[i];
            var token = match(source, offset);
            if (token.IsMatch())
            {
                offset += token.Length;
                if (token.Type == TokenType.NewLine)
                {
                    // for error tracking. it helps establish the column where the error occurred
                    newLineOffsets.Add(token.Offset);
                }

                return token;
            }
        }

        return new Token(offset, 0, TokenType.Error);
    }
}
