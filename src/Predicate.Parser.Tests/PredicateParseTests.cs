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
        _ = Assert.Throws<UnexpectedEndOfSourceException>(() => parser.Parse(String.Empty));
    }

    [Fact]
    public void Returns_Statement()
    {
        var expectedIdentifier = "identifier";
        var property1Name = "property1";
        var property1Value = 1;
        var property2Name = "property2";
        var property2Value = 2;

        var from = $"from {expectedIdentifier}";
        var predicate = $"where {property1Name} = {property1Value} and {property2Name} > {property2Value}";
        var skipValue = 11;
        var takeValue = 22;
        var skip = $"skip {skipValue}";
        var take = $"take {takeValue}";
        var source = $"{from} {predicate} {skip} {take}";

        var statement = parser.Parse(source);
        Assert.Equal(expectedIdentifier, statement.From.Value);
        Assert.Equal(skipValue, statement.Skip?.Value);
        Assert.Equal(takeValue, statement.Take?.Value);
    }
}
