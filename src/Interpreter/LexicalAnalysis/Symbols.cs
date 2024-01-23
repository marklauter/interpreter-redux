namespace Interpreter.LexicalAnalysis;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "so what?")]
public enum Symbols
{
    Undefined = 0,
    Keyword = 100,
    InfixOperator = 200,
    PrefixOperator = 300,
    PostfixOperator = 400,
    Punctuation = 500,
    Identifier = 600,
    Constant = 700,
    String = 800,
    Whitespace = 900,
}
