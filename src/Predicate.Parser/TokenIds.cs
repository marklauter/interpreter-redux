namespace Predicate.Parser;

internal sealed class TokenIds
{
    // literals
    public const int FALSE = 0;
    public const int TRUE = 1;
    public const int FLOATING_POINT_LITERAL = 2;
    public const int INTEGER_LITERAL = 3;
    public const int SCIENTIFIC_NOTATION_LITERAL = 4;
    public const int STRING_LITERAL = 5;
    public const int CHAR_LITERAL = 6;

    // delimiters
    public const int OPEN_PARENTHESIS = '('; // 40
    public const int CLOSE_PARENTHESIS = ')'; // 41

    // comparison operators
    public const int EQUAL = '='; // 61
    public const int GREATER_THAN = '>'; // 62
    public const int LESS_THAN = '<'; // 60
    public const int NOT_EQUAL = 400;
    public const int LESS_THAN_OR_EQUAL = 401;
    public const int GREATER_THAN_OR_EQUAL = 402;
    public const int STARTS_WITH = 405;
    public const int ENDS_WITH = 406;
    public const int CONTAINS = 407;

    // logical operators
    public const int LOGICAL_AND = 403;
    public const int LOGICAL_OR = 404;

    // names
    public const int IDENTIFIER = 500;

    // keywords
    public const int FROM = 300;
    public const int WHERE = 301;
    public const int SKIP = 302;
    public const int TAKE = 303;
}
