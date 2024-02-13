namespace Predicate.Parser.Expressions;

public sealed record Predicate(
    Expression Expression, // from, where, skiptake
    IEnumerable<string> Errors);
