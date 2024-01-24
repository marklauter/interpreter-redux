namespace Interpreter.LexicalAnalysis;

public enum TokenType : int
{
    Undefined = 0,
    Eof = 100,
    Whitespace = 200,
    NewLine = 300,

    Keyword = 1000,
    Identifier = 1100,

    NumericConstant = 2000,
    IntegerConstant = 2100,
    DecimalConstant = 2200,
    StringConstant = 2300,

    InfixOperator = 3000,
    PrefixOperator = 3100,
    PostfixOperator = 3200,

    Punctuation = 4000,
}
