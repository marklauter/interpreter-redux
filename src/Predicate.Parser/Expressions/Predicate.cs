namespace Predicate.Parser.Expressions;

public sealed record Predicate(
    Expression Root,
    IEnumerable<string> Errors);
