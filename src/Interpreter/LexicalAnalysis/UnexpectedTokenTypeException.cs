namespace Interpreter.LexicalAnalysis;

public class UnexpectedTokenTypeException
    : LexicalException
{
    public UnexpectedTokenTypeException()
    {
    }

    public UnexpectedTokenTypeException(string? message) : base(message)
    {
    }

    public UnexpectedTokenTypeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
