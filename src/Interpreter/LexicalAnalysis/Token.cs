namespace Interpreter.LexicalAnalysis;

public readonly record struct Token(
    int Offset,
    int Length,
    TokenType Type,
    string Value);

