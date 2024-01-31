namespace Luthor;

/// <summary>
/// Token types are grouped as flags, so you can perform bitwise checks against meta types like this: 
///   IsLiteral(TokenType type) => type & TokenType.Literal == TokenType.Literal;
/// </summary>
[Flags]
public enum Tokens
    : uint
{
    Error = 0,
    NoMatch = 1u << 0,

    Delimiter = 1u << 1, // meta type - never returned by lexer
    Whitespace = Delimiter | 1u << 2,
    NewLine = Delimiter | 1u << 3,
    EndOfSource = Delimiter | 1u << 4,
    InfixDelimiter = Delimiter | 1u << 5,
    CircumfixDelimiter = Delimiter | 1u << 6,
    OpenCircumfixDelimiter = CircumfixDelimiter | 1u << 7,
    CloseCircumfixDelimiter = CircumfixDelimiter | 1u << 8,

    Operator = 1u << 9, // this includes all kinds: infix, prefix, postfix, circumfix as the type of op is determined by the parser, not the lexer

    Name = 1u << 10,  // meta type - never returned by lexer
    ReservedWord = Name | 1u << 11,
    Identifier = Name | 1u << 12,

    Literal = 1u << 13,  // meta type - never returned by lexer
    NumericLiteral = Literal | 1u << 14,
    StringLiteral = Literal | 1u << 15,
    BooleanLiteral = Literal | 1u << 16,
    CharacterLiteral = Literal | 1u << 18,
    Comment = Literal | 1u << 19,
}
