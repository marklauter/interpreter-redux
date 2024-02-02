using System.Diagnostics.CodeAnalysis;

namespace Luthor.Exceptions;

[ExcludeFromCodeCoverage]
public class LexicalErrorException
    : Exception
{
    public LexicalErrorException()
    {
    }

    public LexicalErrorException(string? message) : base(message)
    {
    }

    public LexicalErrorException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
