namespace Interpreter.LexicalAnalysis;

public enum TokenType : int
{
    Undefined = 0,

    Keyword = 1000,
    Identifier = 1100,

    IntegerConstant = 2000,
    DecimalConstant = 2100,
    StringConstant = 2200,

    InfixOperator = 3000,
    PrefixOperator = 3100,
    PostfixOperator = 3200,

    Punctuation = 4000,

    Whitespace = 5000,
    NewLine = 5100,
}
