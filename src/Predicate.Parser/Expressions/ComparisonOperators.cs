namespace Predicate.Parser.Expressions;

public enum ComparisonOperators
{
    Error = 0,
    Equal = TokenIds.EQUAL,
    NotEqual = TokenIds.NOT_EQUAL,
    LessThan = TokenIds.LESS_THAN,
    GreaterThan = TokenIds.GREATER_THAN,
    LessThanOrEqual = TokenIds.LESS_THAN_OR_EQUAL,
    GreaterThanOrEqual = TokenIds.GREATER_THAN_OR_EQUAL,
    StartsWith = TokenIds.STARTS_WITH,
    EndsWith = TokenIds.ENDS_WITH,
    Contains = TokenIds.CONTAINS,
}
