namespace Interpreter.LexicalAnalysis;

internal sealed class Tokenizer
{
    private enum State
    {
        Whitespace = 0,
        Token = 1,
    }

    // todo: if keywords and operators are provided, then we can build a regex expression to extract symbols
    // token types can be handled by group expressions in the regex
    public IEnumerable<Token> Tokenize(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var state = State.Whitespace;
        var offset = 0;
        for (var i = 0; i < source.Length; ++i)
        {
            switch (state)
            {
                case State.Whitespace:
                    if (!Char.IsWhiteSpace(source[i]))
                    {
                        state = State.Token;
                        offset = i;
                    }

                    break;
                case State.Token:
                    if (Char.IsWhiteSpace(source[i]))
                    {
                        yield return new Token(
                            offset,
                            i - offset);
                        state = State.Whitespace;
                    }

                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        if (state == State.Token)
        {
            yield return new Token(
                offset,
                source.Length - offset);
        }
    }
}
