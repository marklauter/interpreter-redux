namespace Interpreter.LexicalAnalysis;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "so what?")]
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
    String = 900,
    Whitespace = 1000,
}
