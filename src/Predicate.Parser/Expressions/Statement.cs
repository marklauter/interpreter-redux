namespace Predicate.Parser.Expressions;

public sealed record Statement(
    Identifier From,
    Expression Predicate,
    Skip? Skip,
    Take? Take);
