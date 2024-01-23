namespace Interpreter.LexicalAnalysis;

public sealed record Token(
    int Offset,
    int Length,
    TokenType Type,
    string Value);

