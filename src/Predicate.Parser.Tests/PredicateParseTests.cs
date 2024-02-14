using Predicate.Parser.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Predicate.Parser.Tests;

[ExcludeFromCodeCoverage]
public sealed class PredicateParseTests(Parser parser)
{
    private readonly Parser parser = parser ?? throw new ArgumentNullException(nameof(parser));

    [Fact]
    public void Throws_ParseException_When_Source_Is_Empty()
    {
        var ex = Assert.Throws<UnexpectedEndOfSourceException>(() => parser.Parse(String.Empty));
        Assert.Contains(Parser.EndOfSourceError, ex.Message);
    }

    [Fact]
    public void Returns_Statement()
    {
        var expectedIdentifier = "identifier";
        var propertyName = "property";
        var propertyValue = 2;
        var from = $"from {expectedIdentifier}";
        var predicate = $"where {propertyName} == {propertyValue}";
        var source = $"{from} {predicate}";

        var statement = parser.Parse(source);
        Assert.Equal(expectedIdentifier, statement.From.Value);
    }
}
