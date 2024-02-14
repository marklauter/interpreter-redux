namespace Predicate.Parser.Exceptions;

public sealed class UnexpectedEndOfSourceException
    : ParseException
{
    public UnexpectedEndOfSourceException()
    {
    }

    public UnexpectedEndOfSourceException(string? message) : base(message)
    {
    }

    public UnexpectedEndOfSourceException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
