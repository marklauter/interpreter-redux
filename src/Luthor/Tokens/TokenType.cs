namespace Luthor.Tokens;

/// <summary>
/// Token types are grouped as flags, so you can perform bitwise checks against meta types like this: 
///   IsLiteral(TokenType type) => type & TokenType.Literal == TokenType.Literal;
/// </summary>
[Flags]
public enum TokenType : ulong
{
    Error = 0,

    Delimiter = 1ul << 0, // meta type - never returned by lexer
    Whitespace = Delimiter | 1ul << 1,
    NewLine = Delimiter | 1ul << 2,
    EndOfSource = Delimiter | 1ul << 3,
    InfixDelimiter = Delimiter | 1ul << 4,
    CircumfixDelimiter = Delimiter | 1ul << 5,

    Operator = 1ul << 6, // this includes all kinds: infix, prefix, postfix, circumfix as the type of op is determined by the parser, not the lexer

    Name = 1ul << 7,  // meta type - never returned by lexer
    ReservedWord = Name | 1ul << 8,
    Identifier = Name | 1ul << 9,

    Literal = 1ul << 10,  // meta type - never returned by lexer
    NumericLiteral = Literal | 1ul << 11,
    StringLiteral = Literal | 1ul << 12,
    BooleanLiteral = Literal | 1ul << 13,
    CharacterLiteral = Literal | 1ul << 14,
    Comment = Literal | 1ul << 15,
}
