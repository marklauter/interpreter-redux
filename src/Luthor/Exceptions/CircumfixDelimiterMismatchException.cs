using System.Diagnostics.CodeAnalysis;

namespace Luthor.Exceptions;

[ExcludeFromCodeCoverage]
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

