namespace Math.Parser.Exceptions;

public sealed class UnexpectedEndOfSourceException
    : Exception
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
