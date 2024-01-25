namespace Luthor.Tokens;

[Flags]
public enum TokenType : ulong
{
    Error = 0,

    NaturalDelimiter = 1ul << 0,
    Whitespace = NaturalDelimiter | 1ul << 1,
    Eof = NaturalDelimiter | 1ul << 2,
    NewLine = NaturalDelimiter | 1ul << 3,

    SimpleName = 1ul << 4,
    ReservedWord = SimpleName | 1ul << 5,
    Identifier = SimpleName | 1ul << 6,

    Literal = 1ul << 7,
    NumericLiteral = Literal | 1ul << 8,
    StringLiteral = Literal | 1ul << 9,
    BooleanLiteral = Literal | 1ul << 10,
    CharacterLiteral = Literal | 1ul << 11,
    Comment = Literal | 1ul << 12,

    Glyph = 1ul << 13,
    Punctuation = Glyph | 1ul << 14,
    Operator = Glyph | 1ul << 15,
    InfixOperator = Operator | 1ul << 16,
    PrefixOperator = Operator | 1ul << 17,
    PostfixOperator = Operator | 1ul << 18,
    CircufixOperator = Operator | 1ul << 19,
}
