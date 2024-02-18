namespace Lexi;

[Flags]
public enum Tokens
    : uint
{
    Undefined = 0,
    LexError = 1u,
    NoMatch = 1u << 1,

    EndOfSource = 1u << 2,

    Delimiter = 1u << 3,
    Whitespace = Delimiter | 1u << 4,
    NewLine = Delimiter | Whitespace | 1u << 5,
    InfixDelimiter = Delimiter | 1u << 6,
    CircumfixDelimiter = Delimiter | 1u << 7,
    OpenCircumfixDelimiter = CircumfixDelimiter | 1u << 8,
    CloseCircumfixDelimiter = CircumfixDelimiter | 1u << 9,

    Operator = 1u << 10, // this includes infix, prefix, postfix

    Name = 1u << 11,
    Identifier = Name | 1u << 12,
    Keyword = Name | 1u << 13,
    FlowControl = Keyword | 1u << 14,
    Modifier = Keyword | 1u << 15,
    ErrorHandling = Keyword | 1u << 16,
    TypeDeclaration = Keyword | 1u << 17,

    Comment = 1u << 18,

    Literal = 1u << 19,
    NullLiteral = Literal | 1u << 20,
    IntegerLiteral = Literal | 1u << 21,
    FloatingPointLiteral = Literal | 1u << 22,
    ScientificNotationLiteral = Literal | 1u << 23,
    BooleanLiteral = Literal | 1u << 24,
    BooleanTrueLiteral = BooleanLiteral | 1u << 25,
    BooleanFalseLiteral = BooleanLiteral | 1u << 26,
    StringLiteral = Literal | 1u << 27,
    CharacterLiteral = Literal | 1u << 28,
    ArrayLiteral = Literal | 1u << 29,
    ObjectLiteral = Literal | 1u << 30,
}
