using System.Diagnostics.CodeAnalysis;

namespace Luthor.Exceptions;

[ExcludeFromCodeCoverage]
public class SyntaxErrorException
    : LexicalErrorException
{
    public SyntaxErrorException()
    {
    }

    public SyntaxErrorException(string? message) : base(message)
    {
    }

    public SyntaxErrorException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
