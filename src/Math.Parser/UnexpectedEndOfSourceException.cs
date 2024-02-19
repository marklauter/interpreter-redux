namespace Math.Parser;

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
