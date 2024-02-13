using Predicate.Parser.Expressions;
using System.Diagnostics.CodeAnalysis;

namespace Predicate.Parser.Tests;

[ExcludeFromCodeCoverage]
public sealed class PredicateParseTests(Parser parser)
{
    private readonly Parser parser = parser ?? throw new ArgumentNullException(nameof(parser));

    [Fact]
    public void Returns_Error_Predicate_When_String_Is_Empty()
    {
        var predicate = parser.Parse(String.Empty);
        Assert.NotNull(predicate);
        Assert.True(predicate.Expression is ErrorExpression);
        Assert.Contains("unexpected end of source", predicate.Errors);
    }

    [Fact]
    public void Returns_Predicate_With_From_And_Matching_Identifier()
    {
        var expectedIdentifier = "test";
        var source = $"from {expectedIdentifier}";
        var predicate = parser.Parse(source);
        Assert.NotNull(predicate);
        Assert.True(predicate.Expression is From);
        Assert.Empty(predicate.Errors);
    }
}
