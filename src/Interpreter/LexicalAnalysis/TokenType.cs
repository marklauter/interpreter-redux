namespace Interpreter.LexicalAnalysis;

public enum TokenType : int
{
    Undefined = 0,
    Keyword = 100,
    InfixOperator = 200,
    PrefixOperator = 300,
    PostfixOperator = 400,
    Punctuation = 500,
    Identifier = 600,
    IntegerConstant = 700,
    DecimalConstant = 800,
    StringConstant = 900,
    Whitespace = 1000,
}
