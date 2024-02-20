namespace Predicate.Parser.Exceptions;

public sealed class UnexpectedTokenException
    : ParseException
{
    public UnexpectedTokenException()
    {
    }

    public UnexpectedTokenException(string? message) : base(message)
    {
    }

    public UnexpectedTokenException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
