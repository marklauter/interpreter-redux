namespace Interpreter.LexicalAnalysis;

public class TokenKeyNotFoundException
    : LexicalException
{
    public TokenKeyNotFoundException()
    {
    }

    public TokenKeyNotFoundException(string? message) : base(message)
    {
    }

    public TokenKeyNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
