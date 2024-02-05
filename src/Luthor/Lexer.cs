using Luthor.Tokens;

namespace Luthor;

public sealed class Lexer(
    Tokenizers tokenizers,
    string source)
{
    private readonly Tokenizers tokenizers = tokenizers ?? throw new ArgumentNullException(nameof(tokenizers));
    private readonly string source = source ?? throw new ArgumentNullException(nameof(source));

    private int offset;
    private int lastNewLineOffset;
    private int lineNumber = 1;

    public IEnumerable<Token> Tokens()
    {
        var token = NextToken();
        yield return token;
        while (token.Type != TokenType.EndOfSource)
        {
            yield return NextToken();
        };
    }

    public Token NextToken()
    {
        if (offset >= source.Length)
        {
            return new Token(offset, 0, TokenType.EndOfSource, String.Empty);
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
                    lastNewLineOffset = token.Offset; // for error tracking. it helps establish the column where the error occurred
                    ++lineNumber;
                }

                return token;
            }
        }

        var column = offset - lastNewLineOffset;
        return new Token(offset, 0, TokenType.Error, $"{{\"line\": {lineNumber}, \"column\": {column}}}");
    }
}
