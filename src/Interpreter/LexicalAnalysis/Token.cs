namespace Interpreter.LexicalAnalysis;

internal readonly record struct Token(
    int Offset,
    int Length);
