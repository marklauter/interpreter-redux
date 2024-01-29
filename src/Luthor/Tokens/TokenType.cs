namespace Luthor.Tokens;

/// <summary>
/// Token types are grouped as flags, so you can perform bitwise checks against meta types like this: 
///   IsLiteral(TokenType type) => type & TokenType.Literal == TokenType.Literal;
/// </summary>
[Flags]
public enum TokenType : ulong
{
    Error = 0,

    NaturalDelimiter = 1ul << 0, // meta type
    Whitespace = NaturalDelimiter | 1ul << 1,
    Eof = NaturalDelimiter | 1ul << 2,
    NewLine = NaturalDelimiter | 1ul << 3,

    SimpleName = 1ul << 4,  // meta type
    ReservedWord = SimpleName | 1ul << 5,
    Identifier = SimpleName | 1ul << 6,

    Literal = 1ul << 7,  // meta type
    NumericLiteral = Literal | 1ul << 8,
    StringLiteral = Literal | 1ul << 9,
    BooleanLiteral = Literal | 1ul << 10,
    CharacterLiteral = Literal | 1ul << 11,
    Comment = Literal | 1ul << 12,

    Glyph = 1ul << 13,  // meta type
    Punctuation = Glyph | 1ul << 14,
    Operator = Glyph | 1ul << 15,  // meta type
    InfixOperator = Operator | 1ul << 16,
    PrefixOperator = Operator | 1ul << 17,
    PostfixOperator = Operator | 1ul << 18,
    CircumfixOperator = Operator | 1ul << 19,
}
