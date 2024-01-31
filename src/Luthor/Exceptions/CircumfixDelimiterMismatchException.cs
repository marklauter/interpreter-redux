namespace Luthor.Exceptions;

public class CircumfixDelimiterMismatchException
    : SyntaxErrorException
{
    public CircumfixDelimiterMismatchException()
    {
    }

    public CircumfixDelimiterMismatchException(string? message) : base(message)
    {
    }

    public CircumfixDelimiterMismatchException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

